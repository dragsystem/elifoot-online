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
        private readonly INHibernateRepository<Escalacao> escalacaoRepository;

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
            INHibernateRepository<Noticia> noticiaRepository,
            INHibernateRepository<Escalacao> escalacaoRepository)
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
            this.escalacaoRepository = escalacaoRepository;
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

        #region MudarDia

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

                    if (clubevendedor.Jogadores.Count() > 14 || !vendido)
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
                        if (clubevendedor.Jogadores.Count < 15 && !vendido)
                            cancelar = true;

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
                    var jogposicao = clube.Jogadores.Where(x => x.Posicao == pedidojog.Jogador.Posicao).Count();
                    if ((jogposicao > 1 && pedidojog.Jogador.Posicao == 1) || (jogposicao > 1 && pedidojog.Jogador.Posicao == 2) || (jogposicao > 1 && pedidojog.Jogador.Posicao == 4) ||
                        (jogposicao > 1 && pedidojog.Jogador.Posicao == 5) || (jogposicao > 1 && pedidojog.Jogador.Posicao == 6) || (jogposicao > 1 && pedidojog.Jogador.Posicao == 7) ||
                        (jogposicao > 2 && pedidojog.Jogador.Posicao == 3))
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

                        if (g > 1)
                            lstPos.Add(1);
                        if (ld > 1)
                            lstPos.Add(2);
                        if (z > 2)
                            lstPos.Add(3);
                        if (le > 1)
                            lstPos.Add(4);
                        if (v > 2)
                            lstPos.Add(5);
                        if (mo > 2)
                            lstPos.Add(6);
                        if (a > 2)
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
            var partida1 = new Partida();
            var partida2 = new Partida();

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
            for (int i = 0; i < 16; i++) // 16 partidas
            {
                Random rnd = new Random();

                var maxclubes = 32 - (2 * i);
                var clube1 = lstTaca[rnd.Next(0, maxclubes)];
                var clube2 = lstTaca[rnd.Next(0, maxclubes)];

                while (clube1.Id == clube2.Id)
                {
                    clube2 = lstTaca[rnd.Next(0, maxclubes)];
                }

                partida1.Dia = 4;
                partida1.Mao = 1;
                partida1.Rodada = 16;
                partida1.Tipo = "TACA";
                partida1.Clube1 = clube1;
                partida1.Clube2 = clube2;

                partida2.Dia = 7;
                partida2.Mao = 2;
                partida2.Rodada = 16;
                partida2.Tipo = "TACA";
                partida2.Clube1 = clube2;
                partida2.Clube2 = clube1;

                lstTaca.Remove(clube1);
                lstTaca.Remove(clube2);
            }

            partidaRepository.SaveOrUpdate(partida1);
            partidaRepository.SaveOrUpdate(partida2);

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

        #region GerarTaca
        [Transaction]
        public void GerarTaca()
        {
            var partidas = partidaRepository.GetAll().Where(x => x.Tipo == "TACA" && x.Realizada && x.Mao == 2).OrderByDescending(x => x.Rodada);
            var lstTaca = new List<Clube>();
            var rodada = 0;
            var dia1 = 0;
            var dia2 = 0;
            var partida1 = new Partida();
            var partida2 = new Partida();

            foreach (var part in partidas)
            {
                rodada = part.Rodada;
                lstTaca.Add(part.Vencedor);
            }

            // (dia == 4 || dia == 7 || dia == 10 || dia == 13 || dia == 16 || dia == 19 || dia == 22 || dia == 25 || dia == 28)

            if (rodada == 16)
            {
                rodada = 8;
                dia1 = 10;
                dia2 = 13;
            }
            else if (rodada == 8)
            {
                rodada = 4;
                dia1 = 16;
                dia2 = 19;
            }
            else if (rodada == 4)
            {
                rodada = 2;
                dia1 = 22;
                dia2 = 25;
            }
            else
            {
                rodada = 1;
                dia1 = 28;
                dia2 = 31;
            }

            for (int i = 0; i < rodada; i++) //rodada = numero de partidas
            {
                Random rnd = new Random();

                var maxclubes = rodada - (2 * i);
                var clube1 = lstTaca[rnd.Next(0, maxclubes)];
                var clube2 = lstTaca[rnd.Next(0, maxclubes)];

                while (clube1.Id == clube2.Id)
                {
                    clube2 = lstTaca[rnd.Next(0, maxclubes)];
                }

                partida1.Dia = dia1;
                partida1.Mao = 1;
                partida1.Rodada = rodada;
                partida1.Tipo = "TACA";
                partida1.Clube1 = clube1;
                partida1.Clube2 = clube2;

                partida2.Dia = dia2;
                partida2.Mao = 2;
                partida2.Rodada = rodada;
                partida2.Tipo = "TACA";
                partida2.Clube1 = clube2;
                partida2.Clube2 = clube1;

                lstTaca.Remove(clube1);
                lstTaca.Remove(clube2);
            }
            partidaRepository.SaveOrUpdate(partida1);
            partidaRepository.SaveOrUpdate(partida2);
        }
        #endregion

        #region Partida

        [Transaction]
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
                var gol1 = 0;
                var gol2 = 0;

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

                Prob1 = (Prob1 + (Diferenca));
                Prob2 = (Prob2 - (Diferenca));

                Random rnd = new Random();

                var resultado = rnd.Next(0, (Prob1 + Prob2 + ProbEmpate));
                var vencedor = new Clube();

                var placar = rnd.Next(1, 101);

                if (resultado <= Prob1)
                {
                    vencedor = clube1;
                    var dif7 = Convert.ToInt32((Prob1 / 100) * 2);
                    var dif6 = Convert.ToInt32((Prob1 / 100) * 2);
                    var dif5 = Convert.ToInt32((Prob1 / 100) * 4);
                    var dif4 = Convert.ToInt32((Prob1 / 100) * 10);
                    var dif3 = Convert.ToInt32((Prob1 / 100) * 30);
                    var dif2 = Convert.ToInt32((Prob1 / 100) * 50);

                    if (placar <= dif7)
                    {
                        gol2 = rnd.Next(0, 2);
                        gol1 = gol2 + 7;
                    }
                    else if (placar <= dif6)
                    {
                        gol2 = rnd.Next(0, 2);
                        gol1 = gol2 + 6;
                    }
                    else if (placar <= dif5)
                    {
                        gol2 = rnd.Next(0, 3);
                        gol1 = gol2 + 5;
                    }
                    else if (placar <= dif4)
                    {
                        gol2 = rnd.Next(0, 3);
                        gol1 = gol2 + 4;
                    }
                    else if (placar <= dif3)
                    {
                        gol2 = rnd.Next(0, 2);
                        gol1 = gol2 + 3;
                    }
                    else if (placar <= dif2)
                    {
                        gol2 = rnd.Next(0, 3);
                        gol1 = gol2 + 2;
                    }
                    else if (placar > dif2)
                    {
                        gol2 = rnd.Next(0, 3);
                        gol1 = gol2 + 1;
                    }
                }
                else if (resultado <= (Prob1 + ProbEmpate))
                {
                    vencedor = null;
                    if (placar < 35)
                    {
                        gol2 = 0;
                        gol1 = 0;
                    }
                    else if (placar < 70)
                    {
                        gol2 = 1;
                        gol1 = 1;
                    }
                    else if (placar < 90)
                    {
                        gol2 = 2;
                        gol1 = 2;
                    }
                    else if (placar < 98)
                    {
                        gol2 = 3;
                        gol1 = 3;
                    }
                    else if (placar >= 98)
                    {
                        gol2 = 4;
                        gol1 = 4;
                    }
                }
                else
                {
                    vencedor = clube2;
                    var dif7 = Convert.ToInt32((Prob1 / 100) * 2);
                    var dif6 = Convert.ToInt32((Prob1 / 100) * 2);
                    var dif5 = Convert.ToInt32((Prob1 / 100) * 4);
                    var dif4 = Convert.ToInt32((Prob1 / 100) * 10);
                    var dif3 = Convert.ToInt32((Prob1 / 100) * 30);
                    var dif2 = Convert.ToInt32((Prob1 / 100) * 50);

                    if (placar <= dif7)
                    {
                        gol2 = rnd.Next(0, 2);
                        gol1 = gol2 + 7;
                    }
                    else if (placar <= dif6)
                    {
                        gol2 = rnd.Next(0, 2);
                        gol1 = gol2 + 6;
                    }
                    else if (placar <= dif5)
                    {
                        gol2 = rnd.Next(0, 3);
                        gol1 = gol2 + 5;
                    }
                    else if (placar <= dif4)
                    {
                        gol2 = rnd.Next(0, 3);
                        gol1 = gol2 + 4;
                    }
                    else if (placar <= dif3)
                    {
                        gol2 = rnd.Next(0, 2);
                        gol1 = gol2 + 3;
                    }
                    else if (placar <= dif2)
                    {
                        gol2 = rnd.Next(0, 3);
                        gol1 = gol2 + 2;
                    }
                    else if (placar > dif2)
                    {
                        gol2 = rnd.Next(0, 3);
                        gol1 = gol2 + 1;
                    }
                }

                if (partida.Tipo == "TACA" && partida.Mao == 2)
                {
                    var partida1mao = partidaRepository.GetAll().Where(x => x.Rodada == partida.Rodada && x.Mao == 1 && x.Clube1.Id == partida.Clube2.Id && x.Clube2.Id == partida.Clube1.Id).FirstOrDefault();

                    var totalgol1 = partida.Gol1 + partida1mao.Gol2;
                    var totalgol2 = partida.Gol2 + partida1mao.Gol1;

                    if (totalgol1 > totalgol2)
                        vencedor = partida.Clube1;
                    else if (totalgol2 > totalgol1)
                        vencedor = partida.Clube2;
                    else
                    {
                        if (rnd.Next(1, 3) == 1)
                        {
                            vencedor = partida.Clube1;
                            var penal = rnd.Next(0, 3).ToString();
                            if (penal == "0")
                                partida.Penalti = "5 x 4";
                            else if (penal == "1")
                                partida.Penalti = "4 x 3";
                            else
                                partida.Penalti = "3 x 2";
                        }
                        else
                        {
                            vencedor = partida.Clube2;
                            var penal = rnd.Next(0, 3).ToString();
                            if (penal == "0")
                                partida.Penalti = "4 x 5";
                            else if (penal == "1")
                                partida.Penalti = "3 x 4";
                            else
                                partida.Penalti = "2 x 3";
                        }
                    }

                }

                if (gol1 > 0)
                {
                    var totalgols = clube1.Escalacao.Sum(x => x.HGol) + 1;
                    for (int i = 1; i <= gol1; i++)
                    {
                        var gol = new Gol();
                        gol.Clube = clube1;
                        gol.Minuto = rnd.Next(1, 94);
                        gol.Partida = partida;
                        var contagem = 0;
                        var goleador = rnd.Next(1, totalgols);
                        foreach (var jog in clube1.Escalacao.OrderBy(x => x.Posicao))
                        {
                            contagem = contagem + jog.HGol;
                            if (goleador <= contagem)
                            {
                                gol.Jogador = jog.Jogador;
                                break;
                            }
                        }
                        golRepository.SaveOrUpdate(gol);
                    }
                }

                var listgol2 = new List<Gol>();
                if (gol2 > 0)
                {
                    var totalgols = clube2.Escalacao.Sum(x => x.HGol) + 1;
                    for (int i = 1; i <= gol2; i++)
                    {
                        var gol = new Gol();
                        gol.Clube = clube2;
                        gol.Minuto = rnd.Next(1, 94);
                        gol.Partida = partida;
                        var contagem = 0;
                        var goleador = rnd.Next(1, totalgols);
                        foreach (var jog in clube2.Escalacao.OrderBy(x => x.Posicao))
                        {
                            contagem = contagem + jog.HGol;
                            if (goleador <= contagem)
                            {
                                gol.Jogador = jog.Jogador;
                                break;
                            }
                        }
                        golRepository.SaveOrUpdate(gol);
                    }
                }

                var publico = 0;
                publico = clube1.Socios * 10;
                publico = publico - ((publico / 12) * (divisaotabelaRepository.GetAll().Where(x => x.Clube.Id == clube1.Id).FirstOrDefault().Posicao - 1));
                if (clube1.Ingresso > 35)
                    publico = (publico / 100) * 30;
                else if (clube1.Ingresso > 25)
                    publico = (publico / 100) * 50;

                publico = rnd.Next((publico - 3000), publico);
                if (publico > clube1.Estadio)
                    publico = clube1.Estadio;
                else if (publico <= 0)
                    publico = rnd.Next(500, 2000);

                partida.Gol1 = gol1;
                partida.Gol2 = gol2;
                if (partida.Tipo == "TACA" && partida.Mao == 1)
                    partida.Vencedor = null;
                else
                    partida.Vencedor = vencedor;
                partida.Realizada = true;
                partida.Publico = publico;
                partidaRepository.SaveOrUpdate(partida);
            }

            return View();
        }

        #endregion

        #region AtualizaTabela

        [Transaction]
        public void AtualizaTabela()
        {
            foreach (var divisao in divisaoRepository.GetAll())
            {
                var list = new List<DivisaoTabela>();

                foreach (var divisaotabela in divisaotabelaRepository.GetAll().Where(x => x.Divisao.Id == divisao.Id))
                {
                    var partidasdoclube = partidaRepository.GetAll().Where(x => x.Tipo != "TACA" && x.Realizada && (x.Clube1.Id == divisaotabela.Clube.Id || x.Clube2.Id == divisaotabela.Clube.Id));

                    var vitorias = partidasdoclube.Where(x => x.Vencedor.Id == divisaotabela.Clube.Id).Count();
                    var empates = partidasdoclube.Where(x => x.Vencedor == null).Count();
                    var derrotas = partidasdoclube.Where(x => x.Vencedor != null && x.Vencedor.Id != divisaotabela.Clube.Id).Count();
                    var jogos = vitorias + empates + derrotas;
                    var pontos = (vitorias * 3) + empates;
                    var golspro = partidasdoclube.Where(x => x.Clube1.Id == divisaotabela.Clube.Id).Sum(x => x.Gol1) + partidasdoclube.Where(x => x.Clube2.Id == divisaotabela.Clube.Id).Sum(x => x.Gol2);
                    var golscontra = partidasdoclube.Where(x => x.Clube1.Id == divisaotabela.Clube.Id).Sum(x => x.Gol2) + partidasdoclube.Where(x => x.Clube2.Id == divisaotabela.Clube.Id).Sum(x => x.Gol1);

                    divisaotabela.D = derrotas;
                    divisaotabela.E = empates;
                    divisaotabela.GC = golscontra;
                    divisaotabela.GP = golspro;
                    divisaotabela.J = jogos;
                    divisaotabela.Pontos = pontos;
                    divisaotabela.V = vitorias;

                    list.Add(divisaotabela);
                }

                var i = 1;

                foreach (var dt in list.OrderByDescending(x => x.Pontos).ThenByDescending(x => x.V).ThenByDescending(x => x.Saldo).ThenByDescending(x => x.GP))
                {
                    dt.Posicao = i;
                    divisaotabelaRepository.SaveOrUpdate(dt);
                    i++;
                }
            }
        }

        #endregion

        #region EscalaTimes

        [Transaction]
        public void EscalaTimes()
        {
            foreach (var clube in clubeRepository.GetAll())
            {
                foreach (var esc in clube.Escalacao)
                {
                    escalacaoRepository.Delete(esc);
                }

                //GOLEIRO
                var escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 1;
                escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 1).OrderByDescending(x => x.H).FirstOrDefault();
                escalacaoRepository.SaveOrUpdate(escalacao);

                //LATERAL-DIREITO
                if (clube.Formacao.Substring(0, 1) != "3")
                {
                    escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 2;
                    escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 2).OrderByDescending(x => x.H).FirstOrDefault();
                    escalacaoRepository.SaveOrUpdate(escalacao);
                }

                //ZAGUEIROS
                var zagueiros = clube.Jogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).Take(3);
                if (clube.Formacao.Substring(0, 1) == "4")
                    zagueiros = clube.Jogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).Take(2);

                foreach (var zag in zagueiros)
                {
                    escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 3;
                    escalacao.Jogador = zag;
                    escalacaoRepository.SaveOrUpdate(escalacao);
                }

                //LATERAL-ESQUERDO
                if (clube.Formacao.Substring(0, 1) != "3")
                {
                    escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 4;
                    escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 4).OrderByDescending(x => x.H).FirstOrDefault();
                    escalacaoRepository.SaveOrUpdate(escalacao);
                }

                //VOLANTE
                var volantes = clube.Jogadores.Where(x => x.Posicao == 5).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(1, 1)));
                foreach (var vol in volantes)
                {
                    escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 5;
                    escalacao.Jogador = vol;
                    escalacaoRepository.SaveOrUpdate(escalacao);
                }

                //MEIA OFENSIVO
                var meias = clube.Jogadores.Where(x => x.Posicao == 6).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(2, 1)));
                foreach (var mei in meias)
                {
                    escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 6;
                    escalacao.Jogador = mei;
                    escalacaoRepository.SaveOrUpdate(escalacao);
                }

                //ATACANTES
                var atacantes = clube.Jogadores.Where(x => x.Posicao == 7).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(3, 1)));
                foreach (var ata in atacantes)
                {
                    escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 7;
                    escalacao.Jogador = ata;
                    escalacaoRepository.SaveOrUpdate(escalacao);
                }
            }
        }

        #endregion
    }
}
