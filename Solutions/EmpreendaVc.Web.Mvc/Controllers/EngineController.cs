namespace EmpreendaVc.Web.Mvc.Controllers
{
    using System.Web.Mvc;
    using Domain;
    using SharpArch.NHibernate.Web.Mvc;
    using SharpArch.Domain.Commands;
    using SharpArch.NHibernate.Contracts.Repositories;
    using MvcContrib;
    using System.Linq;
    using System.Collections.Generic;
    using MvcContrib.Sorting;
    using MvcContrib.UI.Grid;
    using MvcContrib.Pagination;
    using System;
    using EmpreendaVc.Infrastructure.Queries.Authentication;
    using EmpreendaVc.Infrastructure.Queries.Usuarios;
    using System.Web.Security;
    using EmpreendaVc.Web.Mvc.Controllers.ViewModels;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    public class EngineController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly INHibernateRepository<Jogador> jogadorRepository;
        private readonly INHibernateRepository<Controle> controleRepository;
        private readonly INHibernateRepository<Leilao> leilaoRepository;
        private readonly INHibernateRepository<LeilaoOferta> leilaoofertaRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly INHibernateRepository<Partida> partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<JogadorPedido> jogadorpedidoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;

        public EngineController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            INHibernateRepository<Jogador> jogadorRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<Leilao> leilaoRepository,
            INHibernateRepository<LeilaoOferta> leilaoofertaRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            INHibernateRepository<Partida> partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedido> jogadorpedidoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.jogadorRepository = jogadorRepository;
            this.controleRepository = controleRepository;
            this.leilaoRepository = leilaoRepository;
            this.leilaoofertaRepository = leilaoofertaRepository;
            this.divisaoRepository = divisaoRepository;
            this.partidaRepository = partidaRepository;
            this.golRepository = golRepository;
            this.divisaotabelaRepository = divisaotabelaRepository;
            this.jogadorpedidoRepository = jogadorpedidoRepository;
            this.usuarioofertaRepository = usuarioofertaRepository;
            this.noticiaRepository = noticiaRepository;
        }

        public ActionResult Index(int id)
        {
            try
            {
                var controle = controleRepository.GetAll().FirstOrDefault();

                if (controle.Data.Day < DateTime.Now.Day)
                {
                    MudarDia();
                    if (controle.Dia == 31)
                        MudarAno();
                }
            }
            catch (Exception ex)
            {
                return View(ex.ToString());
                throw;
            }

            return View();
        }

        #region Mudardia

        [Transaction]
        public void MudarDia()
        {
            ////////////////////////////////////////////Atualizar Datas
            var controle = controleRepository.GetAll().FirstOrDefault();
            controle.Data = DateTime.Now;
            controle.Dia = controle.Dia + 1;
            controleRepository.SaveOrUpdate(controle);

            var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Numero;

            ////////////////////////////////////////////Zerar Delay de troca de clube
            foreach (var usuario in usuarioRepository.GetAll().Where(x => x.DelayTroca > 0))
            {
                usuario.DelayTroca = usuario.DelayTroca - 1;
                if (usuario.DelayTroca < 0)
                    usuario.DelayTroca = 0;

                usuarioRepository.SaveOrUpdate(usuario);
            }

            ////////////////////////////////////////////Zerar Ofertas Técnicos
            var semtecclube = new List<Clube>();
            foreach (var usuariooferta in usuarioofertaRepository.GetAll().Where(x => x.Dia < controle.Dia))
            {
                if (semtecclube.Where(x => x.Id == usuariooferta.Clube.Id).Count() == 0)
                    semtecclube.Add(usuariooferta.Clube);



                usuarioofertaRepository.Delete(usuariooferta);
            }
            foreach (var clube in semtecclube)
            {
                clube.ReputacaoAI = 30;
                clubeRepository.SaveOrUpdate(clube);
            }

            ////////////////////////////////////////////Atualizar Finanças e Verifica técnicos
            foreach (var clube in clubeRepository.GetAll())
            {
                if (clube.Usuario != null && clube.Usuario.Reputacao == 0)
                {
                    var tecnicoatual = usuarioRepository.Get(clube.Usuario.Id);

                    var repgeral = clube.Divisao.Numero < ultdivisao ? (80 / clube.Divisao.Numero) : 0;
                    foreach (var tecniconovo in usuarioRepository.GetAll().Where(x => x.Id != tecnicoatual.Id && x.ReputacaoGeral >= repgeral && x.DelayTroca == 0))
                    {
                        var usuariooferta = new UsuarioOferta();

                        usuariooferta.Clube = clube;
                        usuariooferta.Dia = controle.Dia;
                        usuariooferta.Usuario = tecniconovo;

                        usuarioofertaRepository.SaveOrUpdate(usuariooferta);
                    }

                    tecnicoatual.Clube = null;
                    tecnicoatual.DelayTroca = 0;
                    usuarioRepository.SaveOrUpdate(tecnicoatual);

                    clube.Usuario = null;
                }
                else if (clube.Usuario == null && clube.ReputacaoAI < 10)
                {
                    var repgeral = clube.Divisao.Numero < ultdivisao ? (80 / clube.Divisao.Numero) : 0;
                    foreach (var tecniconovo in usuarioRepository.GetAll().Where(x => x.ReputacaoGeral >= repgeral && x.DelayTroca == 0))
                    {
                        var usuariooferta = new UsuarioOferta();

                        usuariooferta.Clube = clube;
                        usuariooferta.Dia = controle.Dia;
                        usuariooferta.Usuario = tecniconovo;

                        usuarioofertaRepository.SaveOrUpdate(usuariooferta);
                    }
                }

                var salarios = clube.Jogadores.Sum(x => x.Salario);
                var renda = clube.Partidas.Where(x => x.Realizada).Last() != null ? clube.Partidas.Where(x => x.Realizada).Last().Publico * clube.Ingresso : 0;
                var socios = clube.Socios * 30;

                clube.Dinheiro = clube.Dinheiro + (renda + socios - salarios);
                clubeRepository.SaveOrUpdate(clube);
            }

            ////////////////////////////////////////////Atualizar Transferencias
            foreach (var leilao in leilaoRepository.GetAll().Where(x => x.Dia < controle.Dia))
            {
                var cancelar = false;
                var vendido = false;

                foreach (var oferta in leilao.Ofertas.Where(x => x.Clube.Dinheiro >= leilao.Valor).OrderByDescending(x => x.Salario))
                {
                    var clubecomprador = clubeRepository.Get(oferta.Clube.Id);
                    var clubevendedor = clubeRepository.Get(leilao.Jogador.Clube.Id);
                    var jogador = leilao.Jogador;

                    if (clubevendedor.Jogadores.Count() > 14 || vendido)
                    {
                        if (clubecomprador.Jogadores.Count() < 25)
                        {
                            clubecomprador.Dinheiro = clubecomprador.Dinheiro - leilao.Valor;
                            clubeRepository.SaveOrUpdate(clubecomprador);

                            jogador.Clube = clubecomprador;
                            jogador.Contrato = true;
                            jogador.Salario = oferta.Salario;
                            jogadorRepository.SaveOrUpdate(jogador);

                            clubevendedor.Dinheiro = clubecomprador.Dinheiro + leilao.Valor;
                            clubeRepository.SaveOrUpdate(clubevendedor);

                            vendido = true;
                            leilao.OfertaVencedora = oferta;
                            leilaoRepository.SaveOrUpdate(leilao);
                        }
                    }
                    else 
                    {
                        cancelar = false;
                        leilaoofertaRepository.Delete(oferta);
                    }
                }

                if (cancelar)
                {
                    leilaoRepository.Delete(leilao);
                }
            }

            ////////////////////////////////////////////ZerarPedidoJogadores
            foreach (var pedidojog in jogadorpedidoRepository.GetAll().Where(x => x.Dia < controle.Dia))
            {
                var clube = clubeRepository.Get(pedidojog.Jogador.Clube.Id);

                if (clube.Jogadores.Count() > 14)
                {
                    Random rnd = new Random();

                    if (rnd.Next(0, 100) < 70)
                    {
                        var leilao = new Leilao();

                        leilao.Dia = controle.Dia;
                        leilao.Espontaneo = true;
                        leilao.Jogador = pedidojog.Jogador;
                        leilao.Valor = pedidojog.Jogador.H * 50000;
                    }
                    else
                    {
                        var jogador = jogadorRepository.Get(pedidojog.Jogador.Id);

                        jogador.Contrato = true;
                        jogador.Salario = pedidojog.Salario;
                    }
                }
                jogadorpedidoRepository.Delete(pedidojog);
            }

            ////////////////////////////////////////////CriarPedidoJogadores e saídas espontaneas
            foreach (var clube in clubeRepository.GetAll())
            {
                Random rnd = new Random();

                var lstjog = clube.Jogadores.ToList();

                if (rnd.Next(1, 3) == 1)
                {
                    if (lstjog.Where(x => !x.Contrato).Count() > 0 && rnd.Next(0, 100) > 70)
                    {
                        var lstjognovo = lstjog.Where(x => !x.Contrato).ToList();
                        var jog = rnd.Next(0, lstjognovo.Count());

                        decimal aumento = (rnd.Next(1, 4)) / 10;

                        var pedidojog = new JogadorPedido();
                        pedidojog.Dia = controle.Dia;
                        pedidojog.Jogador = lstjognovo[jog];
                        pedidojog.Salario = lstjognovo[jog].Salario + (lstjognovo[jog].Salario * aumento);

                        jogadorpedidoRepository.SaveOrUpdate(pedidojog);
                    }
                }
                else
                {
                    if (lstjog.Where(x => !x.Contrato).Count() > 0 && rnd.Next(0, 100) > 50)
                    {
                        var g = lstjog.Where(x => x.Posicao == 1).Count();
                        var ld = lstjog.Where(x => x.Posicao == 2).Count();
                        var z = lstjog.Where(x => x.Posicao == 3).Count();
                        var le = lstjog.Where(x => x.Posicao == 4).Count();
                        var v = lstjog.Where(x => x.Posicao == 5).Count();
                        var mo = lstjog.Where(x => x.Posicao == 6).Count();
                        var a = lstjog.Where(x => x.Posicao == 7).Count();

                        var lstPos = new List<int>();

                        if (g > 2)
                            lstPos.Add(1);
                        if (ld > 1)
                            lstPos.Add(2);
                        if (z > 3)
                            lstPos.Add(3);
                        if (le > 1)
                            lstPos.Add(4);
                        if (v > 2)
                            lstPos.Add(5);
                        if (mo > 2)
                            lstPos.Add(6);
                        if (mo > 3)
                            lstPos.Add(7);

                        var lstjognovo = lstjog.Where(x => !x.Contrato && x.Posicao.IsIn(lstPos)).ToList();
                        var jog = rnd.Next(0, lstjognovo.Count());

                        var leilao = new Leilao();
                        leilao.Dia = controle.Dia;
                        leilao.Espontaneo = true;
                        leilao.Jogador = lstjognovo[jog];
                        leilao.Valor = lstjognovo[jog].H * 50000;
                        leilaoRepository.SaveOrUpdate(leilao);
                    }
                }
            }
        }

        #endregion

        #region MudarAno

        [Transaction]
        public void MudarAno()
        {
            ////////////////////////////////////////////Atualizar Datas
            var controle = controleRepository.GetAll().FirstOrDefault();
            controle.Data = DateTime.Now;
            controle.Dia = 1;
            controle.Ano = controle.Ano + 1;
            controle.Taca = 32;
            controleRepository.SaveOrUpdate(controle);

            ////////////////////////////////////////////Zerar Contratos
            foreach (var jog in jogadorRepository.GetAll())
            {
                jog.Contrato = false;
                jogadorRepository.SaveOrUpdate(jog);
            }

            ////////////////////////////////////////////Gerar Taça
            var itaca = 1;
            var lstTaca = new List<Clube>();
            foreach (var divisaotabela in divisaotabelaRepository.GetAll().OrderBy(x => x.Divisao.Numero).ThenBy(x => x.Posicao))
            {
                var clube = divisaotabela.Clube;
                clube.Taca = true;

                clubeRepository.SaveOrUpdate(clube);

                lstTaca.Add(clube);
                itaca++;

                if (itaca > 32)
                    break;
            }
            for (int i = 1; i < 17; i++) // 16 partidas
            {
                Random rnd = new Random();

                var maxclubes = 34 - (2 * i);
                var clube1 = lstTaca[rnd.Next(0, maxclubes)];
                var clube2 = lstTaca[rnd.Next(0, maxclubes)];

                while (clube1.Id == clube2.Id)
                {
                    clube2 = lstTaca[rnd.Next(0, maxclubes)];
                }

                var partida1 = new Partida();
                partida1.Dia = 4;
                partida1.Mao = 1;
                partida1.Rodada = 16;
                partida1.Tipo = "TACA";
                partida1.Clube1 = clube1;
                partida1.Clube2 = clube2;

                var partida2 = new Partida();
                partida2.Dia = 4;
                partida2.Mao = 1;
                partida2.Rodada = 16;
                partida2.Tipo = "TACA";
                partida2.Clube1 = clube2;
                partida2.Clube2 = clube1;

                lstTaca.Remove(clube1);
                lstTaca.Remove(clube2);
            }

            ////////////////////////////////////////////Alterar Divisao Times
            var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Numero;

            foreach (var divisao in divisaoRepository.GetAll().OrderBy(x => x.Numero))
            {
                var tabela = divisaotabelaRepository.GetAll().Where(x => x.Divisao.Id == divisao.Id).OrderByDescending(x => x.Posicao).ToList();

                var clube1 = tabela[0].Clube; //Campeão
                var clube2 = tabela[1].Clube; //Vice-Campeão
                var clube11 = tabela[10].Clube; //Penúltimo
                var clube12 = tabela[11].Clube; //Último

                ////////////////////////////////////////////Pagar prêmios
                var premio = 6000000 / divisao.Numero;
                clube1.Dinheiro = clube1.Dinheiro + premio;
                clube2.Dinheiro = clube1.Dinheiro + (premio * Convert.ToDecimal(0.30));

                if (divisao.Numero > 1)
                {
                    var promodivisao = divisaoRepository.GetAll().Where(x => x.Numero == (divisao.Numero - 1)).FirstOrDefault();

                    clube1.Divisao = promodivisao;
                    clube2.Divisao = promodivisao;
                }

                if (divisao.Numero < ultdivisao)
                {
                    var rebaixadivisao = divisaoRepository.GetAll().Where(x => x.Numero == (divisao.Numero + 1)).FirstOrDefault();

                    clube11.Divisao = rebaixadivisao;
                    clube12.Divisao = rebaixadivisao;
                }

                clubeRepository.SaveOrUpdate(clube1);
                clubeRepository.SaveOrUpdate(clube2);
                clubeRepository.SaveOrUpdate(clube11);
                clubeRepository.SaveOrUpdate(clube12);
            }

            ////////////////////////////////////////////Zerar Campeonatos
            foreach (var partida in partidaRepository.GetAll())
            {
                partidaRepository.Delete(partida);
            }
            foreach (var tabela in divisaotabelaRepository.GetAll())
            {
                divisaotabelaRepository.Delete(tabela);
            }
            foreach (var gol in golRepository.GetAll())
            {
                golRepository.Delete(gol);
            }

            ////////////////////////////////////////////Gerar Campeonato
            foreach (var divisao in divisaoRepository.GetAll().OrderBy(x => x.Numero))
            {
                var lstclubes = divisao.Clubes.OrderBy(x => x.Nome).ToList();

                ////////////////////////////////////////////Gerar Tabelas
                foreach (var clube in lstclubes)
                {
                    var i = 1;
                    var divisatabela = new DivisaoTabela();
                    divisatabela.Clube = clube;
                    divisatabela.D = 0;
                    divisatabela.Divisao = divisao;
                    divisatabela.E = 0;
                    divisatabela.GC = 0;
                    divisatabela.GP = 0;
                    divisatabela.Pontos = 0;
                    divisatabela.Posicao = i;
                    divisatabela.V = 0;

                    divisaotabelaRepository.SaveOrUpdate(divisatabela);

                    i++;
                }

                ////////////////////////////////////////////Gerar Partidas
                int t1 = 1;
                int t2 = 2;
                int t3 = 12; // ultimo time
                int t4 = 2;
                int t5 = 12;
                int dia = 1;
                for (int i = 1; i < 23; i++) // 22 = rodadas
                {
                    if (dia == 4 || dia == 7 || dia == 10 || dia == 13 || dia == 16 || dia == 19 || dia == 22 || dia == 25 || dia == 28)
                        dia++;

                    t2 = t4;
                    t3 = t5;
                    if ((i % 2) != 0)
                    {
                        var partida = new Partida();
                        partida.Clube1 = lstclubes[t1];
                        partida.Clube2 = lstclubes[t2];
                        partida.Rodada = i;
                        partida.Tipo = "DIVISAO";
                        partida.Dia = dia;
                        partida.Divisao = divisao;
                        partida.Mao = i < 12 ? 1 : 2;
                        partidaRepository.SaveOrUpdate(partida);

                        for (int i2 = 1; i2 < 6; i2++) // 6 = metade do total de times
                        {
                            t2++;
                            if (t2 > 12) // 12 = max de times
                                t2 = 2;

                            partida = new Partida();
                            partida.Clube1 = lstclubes[t3];
                            partida.Clube2 = lstclubes[t2];
                            partida.Rodada = i;
                            partida.Tipo = "DIVISAO";
                            partida.Dia = dia;
                            partida.Divisao = divisao;
                            partida.Mao = i < 12 ? 1 : 2;
                            partidaRepository.SaveOrUpdate(partida);

                            t3--;
                            if (t3 < 2) // 2 = min de times
                                t3 = 12;
                        }
                    }
                    else
                    {
                        var partida = new Partida();
                        partida.Clube1 = lstclubes[t2];
                        partida.Clube2 = lstclubes[t1];
                        partida.Rodada = i;
                        partida.Tipo = "DIVISAO";
                        partida.Dia = dia;
                        partida.Divisao = divisao;
                        partida.Mao = i < 12 ? 1 : 2;
                        partidaRepository.SaveOrUpdate(partida);

                        for (int i2 = 1; i2 < 6; i2++) // 6 = metade do total de times
                        {
                            t2++;
                            if (t2 > 12) // 12 = max de times
                                t2 = 2;

                            partida = new Partida();
                            partida.Clube1 = lstclubes[t2];
                            partida.Clube2 = lstclubes[t3];
                            partida.Rodada = i;
                            partida.Tipo = "DIVISAO";
                            partida.Dia = dia;
                            partida.Divisao = divisao;
                            partida.Mao = i < 12 ? 1 : 2;
                            partidaRepository.SaveOrUpdate(partida);
                            t3--;
                            if (t3 < 2) // 12 = max de times
                                t3 = 12;
                        }
                    }
                    t4--;
                    if (t4 < 2) // 2 = min de times
                        t4 = 12;
                    t5--;
                    if (t5 < 2) // 2 = min de times
                        t5 = 12;

                    dia++;
                }
            }

        }

        #endregion

        #region Partida

        public ActionResult RodaPartida()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            var lstPartidas = partidaRepository.GetAll().Where(x => x.Dia <= controle.Dia);

            var pDefesa = new List<int>();
            pDefesa.Add(1);
            pDefesa.Add(2);
            pDefesa.Add(3);
            pDefesa.Add(4);
            pDefesa.Add(5);

            var pAtaque = new List<int>();
            pAtaque.Add(6);
            pAtaque.Add(7);
            ////////////////////////////////FOR PARTIDAS
            foreach (var partida in lstPartidas)
            {
                var clube1 = clubeRepository.Get(partida.Clube1.Id);
                var clube2 = clubeRepository.Get(partida.Clube2.Id);


                //////////////////////////////////////////////////////////////////
                //ESCALAR TIMES

                var Defesa1 = Convert.ToInt32(clube1.Escalacao.Where(x => x.Posicao.IsIn(pDefesa)).Sum(x => x.H) / clube1.Escalacao.Where(x => x.Posicao.IsIn(pDefesa)).Count());
                var Defesa2 = Convert.ToInt32(clube2.Escalacao.Where(x => x.Posicao.IsIn(pDefesa)).Sum(x => x.H) / clube2.Escalacao.Where(x => x.Posicao.IsIn(pDefesa)).Count());
                var Ataque1 = Convert.ToInt32(clube1.Escalacao.Where(x => x.Posicao.IsIn(pAtaque)).Sum(x => x.H) / clube1.Escalacao.Where(x => x.Posicao.IsIn(pAtaque)).Count());
                var Ataque2 = Convert.ToInt32(clube1.Escalacao.Where(x => x.Posicao.IsIn(pAtaque)).Sum(x => x.H) / clube2.Escalacao.Where(x => x.Posicao.IsIn(pAtaque)).Count());

                var Diferenca = (Defesa1 - Defesa2) + (Ataque1 - Ataque2);

                var Prob1 = 60;
                var Prob2 = 40;
                var ProbEmpate = 0;

                if ((Diferenca * (-1)) > 30)
                    ProbEmpate = 6;
                else if ((Diferenca * (-1)) > 20)
                    ProbEmpate = 12;
                else if ((Diferenca * (-1)) > 10)
                    ProbEmpate = 30;
                else if ((Diferenca * (-1)) >= 0)
                    ProbEmpate = 40;

                Prob1 = (Prob1 + (Diferenca)) - (ProbEmpate / 2);
                Prob2 = (Prob2 + (Diferenca)) - (ProbEmpate / 2);

                Random rnd = new Random();

                var resultado = rnd.Next(1, 101);
                var vencedor = new Clube();

                if (resultado <= Prob1)
                    vencedor = clube1;
                else if (resultado <= (Prob1 + ProbEmpate))
                    vencedor = null;
                else
                    vencedor = clube2;

            }

            return View();
        }

        #endregion
    }
}
