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
    using EmpreendaVc.Infrastructure.Queries.Clubes;
    using EmpreendaVc.Infrastructure.Queries.Partidas;
    using System.Web.Security;
    using EmpreendaVc.Web.Mvc.Controllers.ViewModels;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    public class EngineController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IClubeRepository clubeQueryRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly INHibernateRepository<Jogador> jogadorRepository;
        private readonly INHibernateRepository<Controle> controleRepository;
        private readonly INHibernateRepository<Leilao> leilaoRepository;
        private readonly INHibernateRepository<LeilaoOferta> leilaoofertaRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly IPartidaRepository partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<JogadorPedido> jogadorpedidoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;
        private readonly INHibernateRepository<Escalacao> escalacaoRepository;
        private readonly INHibernateRepository<Historico> historicoRepository;
        private readonly INHibernateRepository<Nome> nomeRepository;

        public EngineController(IUsuarioRepository usuarioRepository,
            IClubeRepository clubeQueryRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            INHibernateRepository<Jogador> jogadorRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<Leilao> leilaoRepository,
            INHibernateRepository<LeilaoOferta> leilaoofertaRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedido> jogadorpedidoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository,
            INHibernateRepository<Escalacao> escalacaoRepository,
            INHibernateRepository<Historico> historicoRepository,
            INHibernateRepository<Nome> nomeRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.clubeQueryRepository = clubeQueryRepository;
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
            this.historicoRepository = historicoRepository;
            this.nomeRepository = nomeRepository;
        }

        //OK - GerarCampeonato(); 
        
        [Transaction]
        public ActionResult Index()
        {
            try
            {
                //var controle = controleRepository.GetAll().FirstOrDefault();

                //GerarCampeonato();
                //AtualizaTabela();
                //if (controle.Data.Day < DateTime.Now.Day)
                //{
                //    //MudarDia();
                //    if (controle.Dia == 31)
                //    {
                //        //MudarAno();
                //    }
                //}
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
        public ActionResult AtualizarDataDia()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            controle.Data = DateTime.Now;
            controle.Dia = controle.Dia + 1;
            controleRepository.SaveOrUpdate(controle);

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZerarDelayUsuario()
        {
            foreach (var usuario in usuarioRepository.UsuarioDelay())
            {
                usuario.DelayTroca = usuario.DelayTroca - 1;
                if (usuario.DelayTroca < 0)
                    usuario.DelayTroca = 0;

                usuarioRepository.SaveOrUpdate(usuario);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZerarOfertaTecnico()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
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

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult AlterarFinancasVerificaTecnicos()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Numero;
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
                        usuariooferta.Dia = controle.Dia + 1;
                        usuariooferta.Usuario = tecniconovo;

                        usuarioofertaRepository.SaveOrUpdate(usuariooferta);
                    }

                    tecnicoatual.Clube = null;
                    tecnicoatual.DelayTroca = 0;
                    tecnicoatual.ReputacaoGeral = tecnicoatual.ReputacaoGeral - 10 < 0 ? 0 : tecnicoatual.ReputacaoGeral - 10;
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
                        usuariooferta.Dia = controle.Dia + 1;
                        usuariooferta.Usuario = tecniconovo;

                        usuarioofertaRepository.SaveOrUpdate(usuariooferta);
                    }
                }

                var salarios = clube.Jogadores.Sum(x => x.Salario);
                var renda = clubeQueryRepository.PartidasClube(clube.Id).Where(x => x.Realizada).Count() > 0 ? clubeQueryRepository.PartidasClube(clube.Id).Where(x => x.Realizada).Last().Publico * clube.Ingresso : 0;
                var socios = clube.Socios * 30;

                clube.Dinheiro = clube.Dinheiro + (renda + socios - salarios);
                clubeRepository.SaveOrUpdate(clube);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult AtualizarTransferencias()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var leilao in leilaoRepository.GetAll().Where(x => x.Dia < controle.Dia))
            {
                var cancelar = false;
                var vendido = false;

                foreach (var oferta in leilaoofertaRepository.GetAll().Where(x => x.Leilao.Id == leilao.Id).OrderByDescending(x => x.Salario).ThenBy(x => x.Clube.Divisao.Numero).ThenByDescending(x => x.Clube.Dinheiro))
                {
                    var clubecomprador = clubeRepository.Get(oferta.Clube.Id);
                    var clubevendedor = clubeRepository.Get(leilao.Jogador.Clube.Id);
                    var jogador = leilao.Jogador;

                    if (clubevendedor.Jogadores.Count() > 14 || !vendido || oferta.Clube.Dinheiro >= leilao.Valor || oferta.Clube.Dinheiro < 200000)
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

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZerarPedidoJogadores()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
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

                            leilao.Dia = controle.Dia + 1;
                            leilao.Espontaneo = true;
                            leilao.Jogador = pedidojog.Jogador;
                            leilao.Clube = pedidojog.Jogador.Clube;
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

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult CriarPedidoSaidasJogadores()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
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
                        pedidojog.Dia = controle.Dia + 1;
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
                        leilao.Dia = controle.Dia + 1;
                        leilao.Espontaneo = true;
                        leilao.Jogador = lstjognovo[jog];
                        leilao.Clube = lstjognovo[jog].Clube;
                        leilao.Valor = lstjognovo[jog].H * 50000;
                        leilaoRepository.SaveOrUpdate(leilao);
                    }
                }
            }
            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult VariarJogadorH()
        {
            var rnd = new Random();
            foreach (var jogador in jogadorRepository.GetAll())
            {
                var variavel = rnd.Next(-2, 3);

                jogador.H = jogador.H + (variavel);

                if (jogador.H > (jogador.HF + 10))
                    jogador.H = jogador.HF + 10;
                else if (jogador.H < (jogador.HF - 10))
                    jogador.H = jogador.HF - 10;

                jogadorRepository.SaveOrUpdate(jogador);
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region MudarAno

        [Transaction]
        public ActionResult AtualizarDataAno()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            controle.Data = DateTime.Now;
            controle.Dia = 1;
            controle.Ano = controle.Ano + 1;
            controle.Taca = 32;
            controleRepository.SaveOrUpdate(controle);

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult GerarHistoricoTaca()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var partidafinaltaca = partidaRepository.GetAll().Where(x => x.Tipo == "TACA" && x.Rodada == 1 && x.Realizada && x.Mao == 2).FirstOrDefault();
            var historico = new Historico();

            if (partidafinaltaca.Vencedor.Usuario != null)
            {
                var usuario = partidafinaltaca.Vencedor.Usuario;
                historico.Usuario = usuario;

                usuario.Reputacao = (usuario.Reputacao + 40) > 50 ? 50 : (usuario.Reputacao + 40);
                usuario.ReputacaoGeral = usuario.ReputacaoGeral + 40;
                usuarioRepository.SaveOrUpdate(usuario);
            }

            historico.Ano = controle.Ano;
            historico.Taca = true;
            historico.Campeao = partidafinaltaca.Vencedor;
            historico.Vice = partidafinaltaca.Vencedor == partidafinaltaca.Clube1 ? partidafinaltaca.Clube2 : partidafinaltaca.Clube1;
            var artilheiro = jogadorRepository.GetAll().OrderByDescending(x => x.Gols.Where(y => y.Partida.Tipo == "TACA")).FirstOrDefault();
            historico.Artilheiro = artilheiro;
            historico.Gols = artilheiro.Gols.Count();
            historicoRepository.SaveOrUpdate(historico);

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZerarContratos()
        {
            foreach (var jog in jogadorRepository.GetAll())
            {
                jog.Contrato = false;
                jogadorRepository.SaveOrUpdate(jog);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult AlterarDivisaoTimes()
        {
            var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Numero;

            foreach (var divisao in divisaoRepository.GetAll().OrderBy(x => x.Numero))
            {
                var tabela = divisaotabelaRepository.GetAll().Where(x => x.Divisao.Id == divisao.Id).OrderByDescending(x => x.Posicao).ToList();

                var clube1 = tabela[0].Clube; //Campeão
                var clube2 = tabela[1].Clube; //Vice-Campeão
                var clube11 = tabela[10].Clube; //Penúltimo
                var clube12 = tabela[11].Clube; //Último

                ////////////////////////////////////////////Gerar reputação
                if (clube1.Usuario != null)
                {
                    var usuario = clube1.Usuario;

                    usuario.Reputacao = (usuario.Reputacao + (40 / divisao.Numero)) > 50 ? 50 : usuario.Reputacao + (40 / divisao.Numero);
                    usuario.ReputacaoGeral = usuario.ReputacaoGeral + (40 / divisao.Numero);
                    usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube1.ReputacaoAI = (clube1.ReputacaoAI + (40 / divisao.Numero)) > 50 ? 50 : clube1.ReputacaoAI + (40 / divisao.Numero);
                }

                if (clube2.Usuario != null)
                {
                    var usuario = clube2.Usuario;

                    usuario.Reputacao = (usuario.Reputacao + (20 / divisao.Numero)) > 50 ? 50 : usuario.Reputacao + (20 / divisao.Numero);
                    usuario.ReputacaoGeral = usuario.ReputacaoGeral + (20 / divisao.Numero);
                    usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube2.ReputacaoAI = (clube2.ReputacaoAI + (20 / divisao.Numero)) > 50 ? 50 : clube2.ReputacaoAI + (20 / divisao.Numero);
                }

                if (clube11.Usuario != null)
                {
                    var usuario = clube11.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 30 < 0 ? 0 : usuario.Reputacao - 30;
                    usuario.ReputacaoGeral = usuario.ReputacaoGeral - 15;
                    usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube11.ReputacaoAI = clube11.ReputacaoAI - 30 < 0 ? 0 : clube11.ReputacaoAI - 30;
                }

                if (clube12.Usuario != null)
                {
                    var usuario = clube12.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 30 < 0 ? 0 : usuario.Reputacao - 30;
                    usuario.ReputacaoGeral = usuario.ReputacaoGeral - 15;
                    usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube12.ReputacaoAI = clube12.ReputacaoAI - 30 < 0 ? 0 : clube12.ReputacaoAI - 30;
                }

                ////////////////////////////////////////////Gerar historico
                var controle = controleRepository.GetAll().FirstOrDefault();
                var historico = new Historico();
                historico.Usuario = clube1.Usuario != null ? clube1.Usuario : null;
                historico.Ano = controle.Ano;
                historico.Divisao = divisao;
                historico.Campeao = clube1;
                historico.Vice = clube2;
                var artilheiro = jogadorRepository.GetAll().OrderByDescending(x => x.Gols.Where(y => y.Partida.Divisao.Id == divisao.Id)).FirstOrDefault();
                historico.Artilheiro = artilheiro;
                historico.Gols = artilheiro.Gols.Count();
                historicoRepository.SaveOrUpdate(historico);

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

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZerarCampeonato()
        {
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
            foreach (var clube in clubeRepository.GetAll())
            {
                clube.Taca = false;
                clubeRepository.SaveOrUpdate(clube);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult GerarCampeonato()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
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
                lstclubes = lstclubes.OrderByDescending(x => x.Dinheiro).ToList();

                int t1 = 0;
                int t2 = 1;
                int t3 = 11; // ultimo time
                int t4 = 1;
                int t5 = 11;
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
                            if (t2 > 11) // 12 = max de times
                                t2 = 1;

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
                            if (t3 < 1) // 2 = min de times
                                t3 = 11;
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
                            if (t2 > 11) // 12 = max de times
                                t2 = 1;

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
                            if (t3 < 1) // 12 = max de times
                                t3 = 11;
                        }
                    }
                    t4--;
                    if (t4 < 1) // 2 = min de times
                        t4 = 11;
                    t5--;
                    if (t5 < 1) // 2 = min de times
                        t5 = 11;

                    dia++;
                }
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ClassificadosTaca()
        {
            var itaca = 1;

            foreach (var divisaotabela in divisaotabelaRepository.GetAll().OrderBy(x => x.Divisao.Numero).ThenBy(x => x.Posicao))
            {
                var clube = clubeRepository.Get(divisaotabela.Clube.Id);
                clube.Taca = true;

                clubeRepository.SaveOrUpdate(clube);

                itaca++;
                if (itaca > 32)
                    break;
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult GerarTacaAno()
        {
            var lstTaca = clubeRepository.GetAll().Where(x => x.Taca).ToList();         


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

                var partida1 = new Partida();
                partida1.Dia = 4;
                partida1.Mao = 1;
                partida1.Rodada = 16;
                partida1.Tipo = "TACA";
                partida1.Clube1 = clube1;
                partida1.Clube2 = clube2;

                var partida2 = new Partida();
                partida2.Dia = 7;
                partida2.Mao = 2;
                partida2.Rodada = 16;
                partida2.Tipo = "TACA";
                partida2.Clube1 = clube2;
                partida2.Clube2 = clube1;

                partidaRepository.SaveOrUpdate(partida1);
                partidaRepository.SaveOrUpdate(partida2);

                lstTaca.Remove(clube1);
                lstTaca.Remove(clube2);
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region GerarTaca

        [Transaction]
        public ActionResult GerarTaca()
        {         
            var partidas = partidaRepository.GetAll().Where(x => x.Tipo == "TACA" && x.Realizada && x.Mao == 2).OrderByDescending(x => x.Rodada);
            var lstTaca = new List<Clube>();
            var rodada = 0;
            var dia1 = 0;
            var dia2 = 0;            

            foreach (var part in partidas)
            {
                rodada = part.Rodada;
                lstTaca.Add(part.Vencedor);
            }

            var controle = controleRepository.GetAll().FirstOrDefault();
            controle.Taca = rodada;
            controleRepository.SaveOrUpdate(controle);

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

                var partida1 = new Partida();
                var partida2 = new Partida();

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

                partidaRepository.SaveOrUpdate(partida1);
                partidaRepository.SaveOrUpdate(partida2);
            }            

            return RedirectToAction("Index", "Engine");
        }
        #endregion

        #region Partida

        [Transaction]
        public ActionResult RodaPartida()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            var lstPartidas = partidaRepository.GetAll().Where(x => x.Dia <= controle.Dia && !x.Realizada).ToList();

            try
            {
                ////////////////////////////////FOR PARTIDAS
                foreach (var partida in lstPartidas)
                {
                    var clube1 = clubeRepository.Get(partida.Clube1.Id);
                    var clube2 = clubeRepository.Get(partida.Clube2.Id);
                    var gol1 = 0;
                    var gol2 = 0;

                    var def1 = clube1.Escalacao.Where(x => x.Posicao == 1 || x.Posicao == 2 || x.Posicao == 3 || x.Posicao == 4 || x.Posicao == 5);
                    var def2 = clube2.Escalacao.Where(x => x.Posicao == 1 || x.Posicao == 2 || x.Posicao == 3 || x.Posicao == 4 || x.Posicao == 5);
                    var ata1 = clube1.Escalacao.Where(x => x.Posicao == 6 || x.Posicao == 7);
                    var ata2 = clube2.Escalacao.Where(x => x.Posicao == 6 || x.Posicao == 7);

                    var Defesa1 = Convert.ToInt32(def1.Sum(x => x.H) / def1.Count());
                    var Defesa2 = Convert.ToInt32(def2.Sum(x => x.H) / def2.Count());
                    var Ataque1 = Convert.ToInt32(ata1.Sum(x => x.H) / ata1.Count());
                    var Ataque2 = Convert.ToInt32(ata2.Sum(x => x.H) / ata2.Count());

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
                        if (clube1.Usuario != null)
                        {
                            var usuario = clube1.Usuario;

                            usuario.Reputacao = usuario.Reputacao + 4 > 50 ? 50 : usuario.Reputacao + 4;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube1.ReputacaoAI = clube1.ReputacaoAI + 4 > 50 ? 50 : clube1.ReputacaoAI + 4;
                        }

                        if (clube2.Usuario != null)
                        {
                            var usuario = clube2.Usuario;

                            usuario.Reputacao = usuario.Reputacao - 6 < 0 ? 0 : usuario.Reputacao - 6;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube2.ReputacaoAI = clube2.ReputacaoAI - 6 < 0 ? 0 : clube2.ReputacaoAI - 6;
                        }

                        var dif7 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                        var dif6 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                        var dif5 = Convert.ToInt32(((double)Prob1 / 100) * 3);
                        var dif4 = Convert.ToInt32(((double)Prob1 / 100) * 5);
                        var dif3 = Convert.ToInt32(((double)Prob1 / 100) * 20);
                        var dif2 = Convert.ToInt32(((double)Prob1 / 100) * 50);

                        if (placar <= dif7)
                        {
                            gol2 = rnd.Next(0, 2);
                            gol1 = gol2 + 7;
                        }
                        else if (placar <= (dif7 + dif6))
                        {
                            gol2 = rnd.Next(0, 2);
                            gol1 = gol2 + 6;
                        }
                        else if (placar <= (dif7 + dif6 + dif5))
                        {
                            gol2 = rnd.Next(0, 3);
                            gol1 = gol2 + 5;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4))
                        {
                            gol2 = rnd.Next(0, 3);
                            gol1 = gol2 + 4;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3))
                        {
                            gol2 = rnd.Next(0, 2);
                            gol1 = gol2 + 3;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                        {
                            gol2 = rnd.Next(0, 3);
                            gol1 = gol2 + 2;
                        }
                        else if (placar > (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                        {
                            gol2 = rnd.Next(0, 3);
                            gol1 = gol2 + 1;
                        }
                    }
                    else if (resultado <= (Prob1 + ProbEmpate))
                    {
                        vencedor = null;
                        if (clube1.Usuario != null)
                        {
                            var usuario = clube1.Usuario;

                            usuario.Reputacao = usuario.Reputacao - 3 < 0 ? 0 : usuario.Reputacao - 3;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube1.ReputacaoAI = clube1.ReputacaoAI - 3 < 0 ? 0 : clube1.ReputacaoAI - 3;
                        }

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
                        if (clube1.Usuario != null)
                        {
                            var usuario = clube1.Usuario;

                            usuario.Reputacao = usuario.Reputacao - 8 < 0 ? 0 : usuario.Reputacao - 8;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube1.ReputacaoAI = clube1.ReputacaoAI - 8 < 0 ? 0 : clube1.ReputacaoAI - 8;
                        }

                        if (clube2.Usuario != null)
                        {
                            var usuario = clube2.Usuario;

                            usuario.Reputacao = usuario.Reputacao + 6 > 50 ? 50 : usuario.Reputacao + 6;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube2.ReputacaoAI = clube2.ReputacaoAI + 6 > 50 ? 50 : clube2.ReputacaoAI + 6;
                        }

                        var dif7 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                        var dif6 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                        var dif5 = Convert.ToInt32(((double)Prob1 / 100) * 3);
                        var dif4 = Convert.ToInt32(((double)Prob1 / 100) * 4);
                        var dif3 = Convert.ToInt32(((double)Prob1 / 100) * 15);
                        var dif2 = Convert.ToInt32(((double)Prob1 / 100) * 40);

                        if (placar <= dif7)
                        {
                            gol1 = rnd.Next(0, 2);
                            gol2 = gol1 + 7;
                        }
                        else if (placar <= (dif7 + dif6))
                        {
                            gol1 = rnd.Next(0, 2);
                            gol2 = gol1 + 6;
                        }
                        else if (placar <= (dif7 + dif6 + dif5))
                        {
                            gol1 = rnd.Next(0, 3);
                            gol2 = gol1 + 5;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4))
                        {
                            gol1 = rnd.Next(0, 3);
                            gol2 = gol1 + 4;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3))
                        {
                            gol1 = rnd.Next(0, 2);
                            gol2 = gol1 + 3;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                        {
                            gol1 = rnd.Next(0, 3);
                            gol2 = gol1 + 2;
                        }
                        else if (placar > (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                        {
                            gol1 = rnd.Next(0, 3);
                            gol2 = gol1 + 1;
                        }
                    }

                    if (partida.Tipo == "TACA" && partida.Mao == 2)
                    {
                        var partida1mao = partidaRepository.GetAll().Where(x => x.Rodada == partida.Rodada && x.Mao == 1 && x.Clube1.Id == partida.Clube2.Id && x.Clube2.Id == partida.Clube1.Id).FirstOrDefault();

                        var teste1 = "1 MAO: " + partida1mao.Clube1.Nome + " " + partida1mao.Gol1 + " x " + partida1mao.Gol2 + " " + partida1mao.Clube2.Nome;
                        var teste2 = "2 MAO: " + partida.Clube1.Nome + " " + gol1 + " x " + gol2 + " " + partida.Clube2.Nome;

                        var totalgol1 = gol1 + partida1mao.Gol2;
                        var totalgol2 = gol2 + partida1mao.Gol1;

                        if (totalgol1 > totalgol2)
                            vencedor = partida.Clube1;
                        else if (totalgol2 > totalgol1)
                            vencedor = partida.Clube2;
                        else
                        {
                            if (partida1mao.Gol2 > gol2)
                                vencedor = partida.Clube1;
                            else if (partida1mao.Gol2 < gol2)
                                vencedor = partida.Clube2;
                            else if (rnd.Next(1, 3) == 1)
                            {
                                vencedor = partida.Clube1;
                                var penal = rnd.Next(0, 3);
                                if (penal == 0)
                                    partida.Penalti = "5 x 4";
                                else if (penal == 1)
                                    partida.Penalti = "4 x 3";
                                else
                                    partida.Penalti = "3 x 2";
                            }
                            else
                            {
                                vencedor = partida.Clube2;
                                var penal = rnd.Next(0, 3);
                                if (penal == 0)
                                    partida.Penalti = "4 x 5";
                                else if (penal == 1)
                                    partida.Penalti = "3 x 4";
                                else
                                    partida.Penalti = "2 x 3";
                            }
                        }

                    }

                    rnd = new Random();

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
                            foreach (var jog in clube1.Escalacao.OrderByDescending(x => x.Posicao))
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

                    rnd = new Random();

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
                            foreach (var jog in clube2.Escalacao.OrderByDescending(x => x.Posicao))
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

                    //publico 
                    var publico = 0;
                    publico = clube1.Socios * 5;
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

                    clube1.Dinheiro = clube1.Dinheiro + (publico * clube1.Ingresso);
                    clubeRepository.SaveOrUpdate(clube1);
                    clubeRepository.SaveOrUpdate(clube2);

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
            }
            catch (Exception ex)
            {
                var teste = ex.ToString();
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region AtualizaTabela

        [Transaction]
        public ActionResult AtualizaTabela()
        {
            foreach (var divisao in divisaoRepository.GetAll())
            {
                var list = new List<DivisaoTabela>();

                foreach (var divisaotabela in divisaotabelaRepository.GetAll().Where(x => x.Divisao.Id == divisao.Id))
                {
                    var partidasdoclube = partidaRepository.GetAll().Where(x => x.Tipo != "TACA" && x.Realizada && (x.Clube1.Id == divisaotabela.Clube.Id || x.Clube2.Id == divisaotabela.Clube.Id));

                    var vitorias = partidasdoclube.Where(x => x.Vencedor != null && x.Vencedor.Id == divisaotabela.Clube.Id).Count();
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

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region EscalaTimes

        [Transaction]
        public ActionResult EscalaTimes()
        {
            try
            {            
                foreach (var clube in clubeRepository.GetAll())
                {
                    foreach (var esc in escalacaoRepository.GetAll().Where(x => x.Clube.Id == clube.Id))
                    {
                        escalacaoRepository.Delete(esc);
                    }

                    //GOLEIRO
                    var escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 1;
                    escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 1).OrderByDescending(x => x.H).FirstOrDefault();
                    if (escalacao.Jogador.H >= 90)
                        escalacao.H = escalacao.Jogador.H + 20;
                    else if (escalacao.Jogador.H >= 80)
                        escalacao.H = escalacao.Jogador.H + 10;
                    else if (escalacao.Jogador.H >= 70)
                        escalacao.H = escalacao.Jogador.H + 5;
                    else
                        escalacao.H = escalacao.Jogador.H;
                    
                    escalacao.HGol = 1;
                    escalacaoRepository.SaveOrUpdate(escalacao);

                    //LATERAL-DIREITO
                    if (clube.Formacao.Substring(0, 1) != "3")
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 2;
                        escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 2).OrderByDescending(x => x.H).FirstOrDefault();
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
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
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        escalacaoRepository.SaveOrUpdate(escalacao);
                    }

                    //LATERAL-ESQUERDO
                    if (clube.Formacao.Substring(0, 1) != "3")
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 4;
                        escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 4).OrderByDescending(x => x.H).FirstOrDefault();
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
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
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
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
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        escalacao.HGol = escalacao.Jogador.H / 2 > 1 ? escalacao.Jogador.H / 2 : 1;
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
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        escalacao.HGol = escalacao.Jogador.H;
                        escalacaoRepository.SaveOrUpdate(escalacao);
                    }
                }
            }
            catch (Exception ex)
            {
                var teste = ex.ToString();
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region GerarJogador

        [Transaction]
        public ActionResult GerarJogador(int id)
        {
            var clube = clubeRepository.Get(id);
            var divisao = clube.Divisao.Numero;
            var nomes = nomeRepository.GetAll();
            var rnd = new Random();

            var g = 2;
            var ld = 2;
            var le = 2;
            var z = 3;
            var v = 3;
            var mo = 3;
            var a = 3;

            for (int i = 0; i < 18; i++)
            {
                var jogador = new Jogador();
                jogador.Clube = clube;

                if (g > 0) {   jogador.Posicao = 1; g--; }
                else if (ld > 0) { jogador.Posicao = 2; ld--; }
                else if (z > 0) { jogador.Posicao = 3; z--; }
                else if (le > 0) { jogador.Posicao = 4; le--; }
                else if (v > 0) { jogador.Posicao = 5; v--; }
                else if (mo > 0) { jogador.Posicao = 6; mo--; }
                else if (a > 0) { jogador.Posicao = 7; a--; }

                var objnome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                if (!objnome.Comum) { objnome = nomes.ElementAt(rnd.Next(0, nomes.Count())); }

                jogador.Nome = objnome.NomeJogador.ToUpper();

                var h = Convert.ToInt32((60 / divisao) / 5) * 5;
                h = h + (5 * rnd.Next(1, 5)) - 10;
                jogador.HF = h > 0 ? h : 1;
                
                jogador.H = jogador.HF;
                jogador.Salario = ((jogador.HF / 2) / divisao) * 1000;
                
                jogadorRepository.SaveOrUpdate(jogador);
            }

            return RedirectToAction("DetalheClube", "Adm", new { id = id });
        }

        #endregion

        public ActionResult PartidaTeste()
        {
            var partida = new Partida();
            var lstGols = new List<Gol>();

            partida.Clube1 = clubeRepository.Get(1);
            partida.Clube2 = clubeRepository.Get(2);
            partida.Mao = 1;
            partida.Rodada = 1;
            partida.Tipo = "DIVISAO";
            partida.Dia = 1;
            partida.Divisao = divisaoRepository.Get(1);

            //var pDefesa = "1,2,3,4,5"; //ALTEREI AQUI

            //var pAtaque = "6,7"; //ALTEREI AQUI

            var clube1 = clubeRepository.Get(partida.Clube1.Id);
            var clube2 = clubeRepository.Get(partida.Clube2.Id);
            var gol1 = 0;
            var gol2 = 0;

            //////////////////////////////////////////////////////////////////
            //ESCALAR TIMES
            

            var def1 = clube1.Escalacao.Where(x => x.Posicao == 1 || x.Posicao == 2 || x.Posicao == 3 || x.Posicao == 4 || x.Posicao == 5);
            var def2 = clube2.Escalacao.Where(x => x.Posicao == 1 || x.Posicao == 2 || x.Posicao == 3 || x.Posicao == 4 || x.Posicao == 5);
            var ata1 = clube1.Escalacao.Where(x => x.Posicao == 6 || x.Posicao == 7);
            var ata2 = clube2.Escalacao.Where(x => x.Posicao == 6 || x.Posicao == 7);

            var Defesa1 = Convert.ToInt32(def1.Sum(x => x.H) / def1.Count());
            var Defesa2 = Convert.ToInt32(def2.Sum(x => x.H) / def2.Count());
            var Ataque1 = Convert.ToInt32(ata1.Sum(x => x.H) / ata1.Count());
            var Ataque2 = Convert.ToInt32(ata2.Sum(x => x.H) / ata2.Count());

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
                if (clube1.Usuario != null)
                {
                    var usuario = clube1.Usuario;

                    usuario.Reputacao = usuario.Reputacao + 4 > 50 ? 50 : usuario.Reputacao + 4;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                if (clube2.Usuario != null)
                {
                    var usuario = clube2.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 6 < 0 ? 0 : usuario.Reputacao - 6;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }

                var dif7 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                var dif6 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                var dif5 = Convert.ToInt32(((double)Prob1 / 100) * 4);
                var dif4 = Convert.ToInt32(((double)Prob1 / 100) * 10);
                var dif3 = Convert.ToInt32(((double)Prob1 / 100) * 30);
                var dif2 = Convert.ToInt32(((double)Prob1 / 100) * 50);

                if (placar <= dif7)
                {
                    gol2 = rnd.Next(0, 2);
                    gol1 = gol2 + 7;
                }
                else if (placar <= (dif7 + dif6))
                {
                    gol2 = rnd.Next(0, 2);
                    gol1 = gol2 + 6;
                }
                else if (placar <= (dif7 + dif6 + dif5))
                {
                    gol2 = rnd.Next(0, 3);
                    gol1 = gol2 + 5;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4))
                {
                    gol2 = rnd.Next(0, 3);
                    gol1 = gol2 + 4;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3))
                {
                    gol2 = rnd.Next(0, 2);
                    gol1 = gol2 + 3;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                {
                    gol2 = rnd.Next(0, 3);
                    gol1 = gol2 + 2;
                }
                else if (placar > (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                {
                    gol2 = rnd.Next(0, 3);
                    gol1 = gol2 + 1;
                }
            }
            else if (resultado <= (Prob1 + ProbEmpate))
            {
                vencedor = null;
                if (clube1.Usuario != null)
                {
                    var usuario = clube1.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 3 < 0 ? 0 : usuario.Reputacao - 3;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }

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
                if (clube1.Usuario != null)
                {
                    var usuario = clube1.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 8 < 0 ? 0 : usuario.Reputacao - 8;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                if (clube2.Usuario != null)
                {
                    var usuario = clube2.Usuario;

                    usuario.Reputacao = usuario.Reputacao + 6 > 50 ? 50 : usuario.Reputacao + 6;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }

                var dif7 = Convert.ToInt32(((double)Prob2 / 100) * 2);
                var dif6 = Convert.ToInt32(((double)Prob2 / 100) * 2);
                var dif5 = Convert.ToInt32(((double)Prob2 / 100) * 4);
                var dif4 = Convert.ToInt32(((double)Prob2 / 100) * 10);
                var dif3 = Convert.ToInt32(((double)Prob2 / 100) * 30);
                var dif2 = Convert.ToInt32(((double)Prob2 / 100) * 50);

                if (placar <= dif7)
                {
                    gol1 = rnd.Next(0, 2);
                    gol2 = gol2 + 7;
                }
                else if (placar <= (dif7 + dif6))
                {
                    gol1 = rnd.Next(0, 2);
                    gol2 = gol2 + 6;
                }
                else if (placar <= (dif7 + dif6 + dif5))
                {
                    gol1 = rnd.Next(0, 3);
                    gol2 = gol2 + 5;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4))
                {
                    gol1 = rnd.Next(0, 3);
                    gol2 = gol2 + 4;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3))
                {
                    gol1 = rnd.Next(0, 2);
                    gol2 = gol2 + 3;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                {
                    gol1 = rnd.Next(0, 3);
                    gol2 = gol2 + 2;
                }
                else if (placar > (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                {
                    gol1 = rnd.Next(0, 3);
                    gol2 = gol2 + 1;
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
                    lstGols.Add(gol);
                    //golRepository.SaveOrUpdate(gol);
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
                    lstGols.Add(gol);
                    //golRepository.SaveOrUpdate(gol);
                }
            }

            //publico 
            var publico = 0;
            publico = clube1.Socios * 5;
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

            partida.Gols = lstGols;

            return View(partida);
        }
    }
}
