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
        private readonly INHibernateRepository<JogadorOferta> jogadorofertaRepository;
        private readonly INHibernateRepository<JogadorLeilao> jogadorleilaoRepository;
        private readonly INHibernateRepository<JogadorLeilaoOferta> jogadorleilaoofertaRepository;
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
        private readonly INHibernateRepository<Sobrenome> sobrenomeRepository;
        private readonly INHibernateRepository<Artilheiro> artilheiroRepository;
        private readonly INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository;
        private readonly INHibernateRepository<Staff> staffRepository;
        private readonly INHibernateRepository<JogadorTeste> jogadortesteRepository;
        private readonly INHibernateRepository<Patrocinio> patrocinioRepository;
        private readonly INHibernateRepository<PatrocinioClube> patrocinioclubeRepository;
        private readonly INHibernateRepository<PatrocinioRecusa> patrociniorecusaRepository;

        public EngineController(IUsuarioRepository usuarioRepository,
            IClubeRepository clubeQueryRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            INHibernateRepository<Jogador> jogadorRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<JogadorOferta> jogadorofertaRepository,
            INHibernateRepository<JogadorLeilao> jogadorleilaoRepository,
            INHibernateRepository<JogadorLeilaoOferta> jogadorleilaoofertaRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedido> jogadorpedidoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository,
            INHibernateRepository<Escalacao> escalacaoRepository,
            INHibernateRepository<Historico> historicoRepository,
            INHibernateRepository<Nome> nomeRepository,
            INHibernateRepository<Sobrenome> sobrenomeRepository,
            INHibernateRepository<Staff> staffRepository,
            INHibernateRepository<Artilheiro> artilheiroRepository,
            INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository,
            INHibernateRepository<JogadorTeste> jogadortesteRepository,
            INHibernateRepository<Patrocinio> patrocinioRepository,
            INHibernateRepository<PatrocinioClube> patrocinioclubeRepository,
            INHibernateRepository<PatrocinioRecusa> patrociniorecusaRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.clubeQueryRepository = clubeQueryRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.jogadorRepository = jogadorRepository;
            this.controleRepository = controleRepository;
            this.jogadorofertaRepository = jogadorofertaRepository;
            this.jogadorleilaoRepository = jogadorleilaoRepository;
            this.jogadorleilaoofertaRepository = jogadorleilaoofertaRepository;
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
            this.sobrenomeRepository = sobrenomeRepository;
            this.artilheiroRepository = artilheiroRepository;
            this.staffRepository = staffRepository;
            this.jogadorhistoricoRepository = jogadorhistoricoRepository;
            this.jogadortesteRepository = jogadortesteRepository;
            this.patrocinioRepository = patrocinioRepository;
            this.patrocinioclubeRepository = patrocinioclubeRepository;
            this.patrociniorecusaRepository = patrociniorecusaRepository;
        }

        //OK - GerarCampeonato(); 

        [Transaction]
        public ActionResult Index()
        {
            try
            {
                var controle = controleRepository.GetAll().FirstOrDefault();
                controle.Manutencao = false;
                controleRepository.SaveOrUpdate(controle);

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

        public ActionResult MudarDia()
        {
            ViewBag.AtualizarDataDia = false;
            ViewBag.EscalaTimes = false;
            ViewBag.RodaPartida = false;
            ViewBag.AtualizaTabela = false;
            ViewBag.GerarTaca = false;
            ViewBag.ZerarDelayUsuario = false;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = false;
            ViewBag.VerificaTecnicos = false;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
        }

        #region MudarDia

        [Transaction]
        public ActionResult AtualizarDataDia()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            controle.Data = DateTime.Now;
            controle.Dia = controle.Dia + 1 <= controle.DiaMax ? controle.Dia + 1 : 1;
            controleRepository.SaveOrUpdate(controle);

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = false;
            ViewBag.RodaPartida = false;
            ViewBag.AtualizaTabela = false;
            ViewBag.GerarTaca = false;
            ViewBag.ZerarDelayUsuario = false;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = false;
            ViewBag.VerificaTecnicos = false;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
        }

        [Transaction]
        public ActionResult EscalaTimes()
        {
            try
            {
                foreach (var clube in clubeRepository.GetAll())
                {
                    var lstnova = new List<Escalacao>();

                    var lstJogadores = clube.Jogadores.Where(x => x.Lesionado == 0);

                    //GOLEIRO
                    var escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 1;
                    if (clube.Usuario != null)
                        escalacao.Jogador = lstJogadores.Where(x => x.Posicao == 1).OrderByDescending(x => x.TreinoMedia).FirstOrDefault();
                    else
                        escalacao.Jogador = lstJogadores.Where(x => x.Posicao == 1).OrderByDescending(x => x.H).FirstOrDefault();

                    if (escalacao.Jogador.H >= 90)
                        escalacao.H = escalacao.Jogador.H + 20;
                    else if (escalacao.Jogador.H >= 80)
                        escalacao.H = escalacao.Jogador.H + 10;
                    else if (escalacao.Jogador.H >= 70)
                        escalacao.H = escalacao.Jogador.H + 5;
                    else
                        escalacao.H = escalacao.Jogador.H;

                    //escalacao.HGol = 1;
                    lstnova.Add(escalacao);

                    //LATERAL-DIREITO
                    if (clube.Formacao.Substring(0, 1) != "3")
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 2;
                        if (clube.Usuario != null)
                            escalacao.Jogador = lstJogadores.Where(x => x.Posicao == 2).OrderByDescending(x => x.TreinoMedia).FirstOrDefault();
                        else
                            escalacao.Jogador = lstJogadores.Where(x => x.Posicao == 2).OrderByDescending(x => x.H).FirstOrDefault();

                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        //escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        lstnova.Add(escalacao);
                    }

                    //ZAGUEIROS
                    var zagueiros = lstJogadores.Where(x => x.Posicao == 3);
                    if (clube.Usuario != null)
                        zagueiros = zagueiros.OrderByDescending(x => x.TreinoMedia).Take(3);
                    else
                        zagueiros = zagueiros.OrderByDescending(x => x.H).Take(3);

                    if (clube.Formacao.Substring(0, 1) == "4")
                        zagueiros = lstJogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).Take(2);

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

                        //escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        lstnova.Add(escalacao);
                    }

                    //LATERAL-ESQUERDO
                    if (clube.Formacao.Substring(0, 1) != "3")
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 4;
                        if (clube.Usuario != null)
                            escalacao.Jogador = lstJogadores.Where(x => x.Posicao == 4).OrderByDescending(x => x.TreinoMedia).FirstOrDefault();
                        else
                            escalacao.Jogador = lstJogadores.Where(x => x.Posicao == 4).OrderByDescending(x => x.H).FirstOrDefault();
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        //escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        lstnova.Add(escalacao);
                    }

                    //VOLANTE
                    var volantes = lstJogadores.Where(x => x.Posicao == 5);
                    if (clube.Usuario != null)
                        volantes = volantes.OrderByDescending(x => x.TreinoMedia);
                    else
                        volantes = volantes.OrderByDescending(x => x.H);

                    volantes = volantes.Take(Convert.ToInt32(clube.Formacao.Substring(1, 1)));

                    if (clube.Formacao.Substring(0, 1) == "4")
                        zagueiros = lstJogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).Take(2);

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

                        //escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        lstnova.Add(escalacao);
                    }

                    //MEIA OFENSIVO
                    var meias = lstJogadores.Where(x => x.Posicao == 6);
                    if (clube.Usuario != null)
                        meias = meias.OrderByDescending(x => x.TreinoMedia);
                    else
                        meias = meias.OrderByDescending(x => x.H);

                    meias = meias.Take(Convert.ToInt32(clube.Formacao.Substring(2, 1)));

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

                        //escalacao.HGol = escalacao.Jogador.H / 2 > 1 ? escalacao.Jogador.H / 2 : 1;
                        lstnova.Add(escalacao);
                    }

                    //ATACANTES
                    var atacantes = lstJogadores.Where(x => x.Posicao == 7);
                    if (clube.Usuario != null)
                        atacantes = atacantes.OrderByDescending(x => x.TreinoMedia);
                    else
                        atacantes = atacantes.OrderByDescending(x => x.H);

                    atacantes = atacantes.Take(Convert.ToInt32(clube.Formacao.Substring(3, 1)));

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

                        //escalacao.HGol = escalacao.Jogador.H;
                        lstnova.Add(escalacao);
                    }

                    var i = 0;
                    var lstescalacao = escalacaoRepository.GetAll().Where(x => x.Clube.Id == clube.Id).ToList();

                    foreach (var item in lstescalacao)
                    {
                        item.Posicao = lstnova[i].Posicao;
                        if (item.Jogador != null && clube.Usuario != null)
                        {
                            item.H = Util.Util.RetornaHabilidadePosicao(item.Jogador, item.Posicao);
                        }
                        else
                        {
                            item.Jogador = lstnova[i].Jogador;
                            item.H = Util.Util.RetornaHabilidadePosicao(item.Jogador, item.Posicao);
                        }
                        escalacaoRepository.SaveOrUpdate(item);

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                var teste = ex.ToString();
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = false;
            ViewBag.AtualizaTabela = false;
            ViewBag.GerarTaca = false;
            ViewBag.ZerarDelayUsuario = false;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = false;
            ViewBag.VerificaTecnicos = false;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
        }

        [Transaction]
        public ActionResult RodaPartida()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            var lstPartidas = partidaRepository.GetAll().Where(x => x.Dia < controle.Dia && !x.Realizada).ToList();

            try
            {
                ////////////////////////////////FOR PARTIDAS
                foreach (var partida in lstPartidas)
                {
                    //partidaRepository.LimparGols(partida.Id);

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

                    var Diferenca = (Defesa1 - Ataque2) + (Defesa2 - Ataque1);

                    var Prob1 = 55;
                    var Prob2 = 45;
                    var ProbEmpate = 0;
                    var dif7 = 0;
                    var dif6 = 0;
                    var dif5 = 0;
                    var dif4 = 0;
                    var dif3 = 0;
                    var dif2 = 0;

                    if ((Diferenca * -1) < 10)
                        ProbEmpate = 50;
                    else if ((Diferenca * -1) < 20)
                        ProbEmpate = 40;
                    else if ((Diferenca * -1) < 30)
                        ProbEmpate = 20;
                    else if ((Diferenca * -1) >= 30)
                        ProbEmpate = 10;

                    Prob1 = (Prob1 + (Diferenca));
                    Prob2 = (Prob2 - (Diferenca));

                    Random rnd = new Random();

                    var resultado = rnd.Next(0, (Prob1 + Prob2 + ProbEmpate));
                    var vencedor = new Clube();

                    var placar = rnd.Next(1, 101);

                    //TIME 1 VENCEU
                    if (resultado <= Prob1)
                    {
                        vencedor = clube1;
                        if (clube1.Usuario != null)
                        {
                            var usuario = clube1.Usuario;
                            usuario.Reputacao = usuario.Reputacao + 4 > 50 ? 50 : usuario.Reputacao + 4;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else { clube1.ReputacaoAI = clube1.ReputacaoAI + 4 > 50 ? 50 : clube1.ReputacaoAI + 4; }

                        if (clube2.Usuario != null)
                        {
                            var usuario = clube2.Usuario;
                            usuario.Reputacao = usuario.Reputacao - 6 < 0 ? 0 : usuario.Reputacao - 6;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else { clube2.ReputacaoAI = clube2.ReputacaoAI - 6 < 0 ? 0 : clube2.ReputacaoAI - 6; }

                        if (Diferenca <= -30)
                        {
                            dif7 = 0; dif6 = 0; dif5 = 1; dif4 = 2; dif3 = 5; dif2 = 12;
                        }
                        else if (Diferenca < -20)
                        {
                            dif7 = 0; dif6 = 0; dif5 = 1; dif4 = 4; dif3 = 8; dif2 = 17;
                        }
                        else if (Diferenca < -10)
                        {
                            dif7 = 0; dif6 = 1; dif5 = 2; dif4 = 5; dif3 = 10; dif2 = 25;
                        }
                        else if (Diferenca < 10)
                        {
                            dif7 = 1; dif6 = 2; dif5 = 4; dif4 = 6; dif3 = 10; dif2 = 27;
                        }
                        else if (Diferenca < 20)
                        {
                            dif7 = 1; dif6 = 2; dif5 = 5; dif4 = 8; dif3 = 12; dif2 = 30;
                        }
                        else if (Diferenca < 30)
                        {
                            dif7 = 2; dif6 = 3; dif5 = 6; dif4 = 12; dif3 = 20; dif2 = 35;
                        }
                        else if (Diferenca >= 30)
                        {
                            dif7 = 3; dif6 = 4; dif5 = 8; dif4 = 15; dif3 = 25; dif2 = 35;
                        }

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
                            gol2 = rnd.Next(0, 2);
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
                    //resultado empate
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
                    //TIME 2 VENCEU
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

                        if (Diferenca <= -30)
                        {
                            dif7 = 1; dif6 = 2; dif5 = 5; dif4 = 15; dif3 = 25; dif2 = 35;
                        }
                        else if (Diferenca < -20)
                        {
                            dif7 = 1; dif6 = 2; dif5 = 4; dif4 = 10; dif3 = 20; dif2 = 35;
                        }
                        else if (Diferenca < -10)
                        {
                            dif7 = 0; dif6 = 1; dif5 = 2; dif4 = 5; dif3 = 10; dif2 = 25;
                        }
                        else if (Diferenca < 10)
                        {
                            dif7 = 0; dif6 = 2; dif5 = 4; dif4 = 6; dif3 = 10; dif2 = 27;
                        }
                        else if (Diferenca < 20)
                        {
                            dif7 = 0; dif6 = 1; dif5 = 3; dif4 = 6; dif3 = 12; dif2 = 30;
                        }
                        else if (Diferenca < 30)
                        {
                            dif7 = 1; dif6 = 2; dif5 = 4; dif4 = 8; dif3 = 25; dif2 = 35;
                        }
                        else if (Diferenca >= 30)
                        {
                            dif7 = 1; dif6 = 2; dif5 = 5; dif4 = 10; dif3 = 20; dif2 = 35;
                        }

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
                            gol1 = rnd.Next(0, 2);
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

                    ///FIM RESULTADO DA PARTIDA.

                    //SE FOR TAÇA E PARTIDA QUE DECIDE
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

                    ///FIM TAÇA

                    var lstgoleadores1 = new List<GolPossibilidadeView>();
                    var lstgoleadores2 = new List<GolPossibilidadeView>();

                    //ATUALIZA TIMES. JOGOS, H E CONDIÇÃO
                    foreach (var escalacao in clube1.Escalacao)
                    {
                        var jogador = escalacao.Jogador;
                        jogador.Jogos = jogador.Jogos + 1;
                        jogador.H = jogador.H - 1;

                        if (jogador.H < (jogador.HF - 10))
                            jogador.H = jogador.HF - 10;

                        if (jogador.H < 1)
                            jogador.H = 1;

                        if (jogador.Posicao != 1 && !jogador.Temporario && jogador.Clube != null && jogador.Clube.Usuario != null)
                            jogador.Condicao = jogador.Condicao - 15;

                        jogadorRepository.SaveOrUpdate(jogador);

                        if (gol1 > 0)
                        {
                            var gp = new GolPossibilidadeView();
                            var comp = lstgoleadores1.Count() > 0 ? lstgoleadores1.OrderByDescending(x => x.P).FirstOrDefault().P : 0;
                            gp.Jogador = jogador;
                            if (escalacao.Posicao == 1)
                                gp.P = 0;
                            else
                                gp.P = comp + escalacao.HGol;

                            lstgoleadores1.Add(gp);
                        }
                    }

                    foreach (var escalacao in clube2.Escalacao)
                    {
                        var jogador = escalacao.Jogador;
                        jogador.Jogos = jogador.Jogos + 1;
                        jogador.H = jogador.H - 1;

                        if (jogador.H < (jogador.HF - 10))
                            jogador.H = jogador.HF - 10;

                        if (jogador.H < 1)
                            jogador.H = 1;

                        if (jogador.Posicao != 1 && !jogador.Temporario && jogador.Clube != null && jogador.Clube.Usuario != null)
                            jogador.Condicao = jogador.Condicao - 15;

                        jogadorRepository.SaveOrUpdate(jogador);

                        if (gol2 > 0)
                        {
                            var gp = new GolPossibilidadeView();
                            var comp = lstgoleadores2.Count() > 0 ? lstgoleadores2.OrderByDescending(x => x.P).FirstOrDefault().P : 0;
                            gp.Jogador = jogador;
                            if (escalacao.Posicao == 1)
                                gp.P = 0;
                            else
                                gp.P = comp + escalacao.HGol;

                            lstgoleadores2.Add(gp);
                        }
                    }

                    //DECIDE QUEM FEZ OS GOLS
                    rnd = new Random();
                    if (gol1 > 0)
                    {
                        for (int i = 1; i <= gol1; i++)
                        {
                            var gol = new Gol();
                            gol.Clube = clube1;
                            gol.Minuto = rnd.Next(1, 94);
                            gol.Partida = partida;
                            var goleador = rnd.Next(1, lstgoleadores1.OrderByDescending(x => x.P).FirstOrDefault().P + 1);
                            gol.Jogador = lstgoleadores1.FirstOrDefault(x => goleador <= x.P).Jogador;

                            golRepository.SaveOrUpdate(gol);
                        }
                    }

                    rnd = new Random();
                    rnd.Next(1, 100);

                    var listgol2 = new List<Gol>();
                    if (gol2 > 0)
                    {
                        for (int i = 1; i <= gol2; i++)
                        {
                            var gol = new Gol();
                            gol.Clube = clube2;
                            gol.Minuto = rnd.Next(1, 94);
                            gol.Partida = partida;
                            var goleador = rnd.Next(1, lstgoleadores2.OrderByDescending(x => x.P).FirstOrDefault().P + 1);
                            gol.Jogador = lstgoleadores2.FirstOrDefault(x => goleador <= x.P).Jogador;

                            golRepository.SaveOrUpdate(gol);
                        }
                    }

                    //publico 
                    var publico = 0;
                    publico = clube1.Socios * 5;
                    publico = publico - ((publico / 12) * (divisaotabelaRepository.GetAll().Where(x => x.Clube.Id == clube1.Id).FirstOrDefault().Posicao - 1));
                    if (clube1.Ingresso > 35)
                        publico = publico / 2;
                    else if (clube1.Ingresso > 25)
                        publico = (publico / 4) * 3;

                    if (partida.Tipo == "TACA" && partida.Rodada < 2)
                        publico = publico * 2;
                    else if (partida.Tipo == "TACA" && partida.Rodada < 4)
                        publico = publico + (publico / 2);

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

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = false;
            ViewBag.GerarTaca = false;
            ViewBag.ZerarDelayUsuario = false;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = false;
            ViewBag.VerificaTecnicos = false;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
            //return RedirectToAction("Index", "Engine");
        }

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

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = false;
            ViewBag.ZerarDelayUsuario = false;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = false;
            ViewBag.VerificaTecnicos = false;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
            //return View("Index");
        }

        [Transaction]
        public ActionResult GerarTaca()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            var lstTaca = new List<Clube>();
            var rodada = controle.Taca / 2;

            if (controle.Taca > 2)
            {
                var partidas = partidaRepository.GetAll().Where(x => x.Tipo == "TACA" && x.Realizada && x.Mao == 2 && x.Rodada == rodada).OrderByDescending(x => x.Rodada);

                var dia1 = 0;
                var dia2 = 0;

                if (partidas.Count() > 0)
                {
                    foreach (var part in partidas)
                    {
                        lstTaca.Add(part.Vencedor);
                    }

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
                        dia2 = 32;
                    }

                    for (int i = 0; i < rodada; i++) //rodada = numero de partidas
                    {
                        Random rnd = new Random();

                        var maxclubes = (rodada * 2) - (2 * i);
                        var clube1 = lstTaca[rnd.Next(0, maxclubes)];
                        var clube2 = lstTaca[rnd.Next(0, maxclubes)];

                        while (clube1.Id == clube2.Id)
                        {
                            clube2 = lstTaca[rnd.Next(0, maxclubes)];
                        }

                        var partida1 = new Partida();
                        partida1.Dia = dia1;
                        partida1.Mao = 1;
                        partida1.Rodada = rodada;
                        partida1.Tipo = "TACA";
                        partida1.Clube1 = clube1;
                        partida1.Clube2 = clube2;

                        var partida2 = new Partida();
                        partida2.Dia = dia2;
                        partida2.Mao = 2;
                        partida2.Rodada = rodada;
                        partida2.Tipo = "TACA";
                        partida2.Clube1 = clube2;
                        partida2.Clube2 = clube1;

                        partidaRepository.SaveOrUpdate(partida1);
                        partidaRepository.SaveOrUpdate(partida2);

                        lstTaca.Remove(clube1);
                        lstTaca.Remove(clube2);
                    }
                }
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = false;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = false;
            ViewBag.VerificaTecnicos = false;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
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

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = false;
            ViewBag.VerificaTecnicos = false;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
        }

        [Transaction]
        public ActionResult UpdateFinancas()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var clube in clubeRepository.GetAll())
            {
                var salarios = clube.Jogadores.Sum(x => x.Salario);
                var socios = clube.Socios * 30;
                decimal staff = 0;
                decimal patrocinio = 0;

                if (clube.Usuario != null)
                    staff = clube.Usuario.Staffs.Sum(x => x.Salario);

                if (clube.PatrocinioClubes.Count() > 0)
                    patrocinio = clube.PatrocinioClubes.Sum(x => x.Valor) / controle.DiaMax;

                clube.Socios = clube.Socios + (7 - clube.DivisaoTabelas.FirstOrDefault().Posicao);
                clube.Dinheiro = clube.Dinheiro + (socios + patrocinio) - (salarios + staff);
                clubeRepository.SaveOrUpdate(clube);
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = true;
            ViewBag.VerificaTecnicos = false;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
        }

        [Transaction]
        public ActionResult VerificaTecnicos()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Numero;
            foreach (var clube in clubeRepository.GetAll())
            {
                if (clube.Usuario != null && clube.Usuario.Reputacao == 0)
                {
                    var tecnicoatual = usuarioRepository.Get(clube.Usuario.Id);

                    tecnicoatual.Clube = null;
                    tecnicoatual.Reputacao = 30;
                    tecnicoatual.DelayTroca = 0;
                    tecnicoatual.ReputacaoGeral = tecnicoatual.ReputacaoGeral - 10 < 0 ? 0 : tecnicoatual.ReputacaoGeral - 10;
                    tecnicoatual.IdUltimoClube = clube.Id;
                    usuarioRepository.SaveOrUpdate(tecnicoatual);

                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Você foi despedido do " + Util.Util.LinkaClube(clube) + ", pois a diretoria não estava satisfeita com seu trabalho.<br />Procure outro clube para dirigir.";
                    noticia.Usuario = tecnicoatual;
                    noticiaRepository.SaveOrUpdate(noticia);

                    var repgeral = clube.Divisao.Numero < ultdivisao ? (80 / clube.Divisao.Numero) : 0;
                    foreach (var tecniconovo in usuarioRepository.GetAll().Where(x => x.ReputacaoGeral >= repgeral && x.DelayTroca == 0 && x.IdUltimoClube != clube.Id))
                    {
                        noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = Util.Util.LinkaClube(clube) + " despediu o técnico e está a procura de um novo técnico.";
                        noticia.Usuario = tecniconovo;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    clube.Usuario = null;
                }
                else if (clube.Usuario == null && clube.ReputacaoAI < 5)
                {
                    var repgeral = clube.Divisao.Numero < ultdivisao ? (80 / clube.Divisao.Numero) : 0;
                    foreach (var tecniconovo in usuarioRepository.GetAll().Where(x => x.ReputacaoGeral >= repgeral && x.DelayTroca == 0 && x.IdUltimoClube != clube.Id))
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = Util.Util.LinkaClube(clube) + " despediu o técnico e está a procura de um novo técnico.";
                        noticia.Usuario = tecniconovo;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    clube.ReputacaoAI = 30;
                    clube.Formacao = "4222";
                }
                clubeRepository.SaveOrUpdate(clube);
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = true;
            ViewBag.VerificaTecnicos = true;
            ViewBag.AtualizarTransferencias = false;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
            //return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult AtualizarTransferencias()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var lstvendidos = new List<Jogador>();
            var lstposvendidos = new List<PosicaoCompradaView>();
            var lstleilao = new List<JogadorLeilaoOferta>();

            foreach (var jogadoroferta in jogadorofertaRepository.GetAll().Where(x => x.Dia < controle.Dia && x.Estagio == 2).OrderByDescending(x => x.Jogador.H).ThenByDescending(x => x.Pontos).ThenByDescending(x => x.Salario))
            {
                var jogador = jogadoroferta.Jogador;
                var clubecomprador = clubeRepository.Get(jogadoroferta.Clube.Id);
                var clubevendedor = jogador.Clube != null ? clubeRepository.Get(jogador.Clube.Id) : null;

                if (((clubevendedor != null && clubevendedor.Jogadores.Where(x => !x.Temporario).Count() > 14) || clubevendedor == null) && lstvendidos.Where(x => x.Id == jogador.Id).Count() == 0 && lstposvendidos.Where(x => x.Clube.Id == clubecomprador.Id && x.Posicao == jogador.Posicao).Count() == 0 && clubecomprador.Dinheiro >= jogadoroferta.Valor && jogadoroferta.Pontos > 0)
                {
                    clubecomprador.Dinheiro = clubecomprador.Dinheiro - jogadoroferta.Valor;
                    clubeRepository.SaveOrUpdate(clubecomprador);

                    if (clubevendedor != null)
                    {
                        clubevendedor.Dinheiro = clubecomprador.Dinheiro + jogadoroferta.Valor;
                        clubeRepository.SaveOrUpdate(clubevendedor);

                        var historico = new JogadorHistorico();
                        historico.Ano = controle.Ano;
                        historico.Clube = clubevendedor;
                        historico.Gols = jogador.Gols.Where(x => x.Clube.Id == clubevendedor.Id).Count();
                        historico.Jogador = jogador;
                        historico.Jogos = jogador.Jogos;
                        historico.NotaMedia = jogador.NotaMedia > 0.0 ? jogador.NotaMedia : 0.00;
                        historico.Valor = jogadoroferta.Valor;
                        jogadorhistoricoRepository.SaveOrUpdate(historico);
                    }

                    jogador.Clube = clubecomprador;
                    jogador.Contrato = jogadoroferta.Contrato;
                    jogador.Salario = jogadoroferta.Salario;
                    jogador.Situacao = 1;
                    jogador.Satisfacao = 0;
                    jogador.Jogos = 0;
                    jogador.NotaTotal = 0;
                    jogador.NotaUlt = 0;
                    jogador.Treinos = 0;
                    jogador.TreinoUlt = 0;
                    jogador.TreinoTotal = 0;
                    jogadorRepository.SaveOrUpdate(jogador);

                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        if (clubevendedor != null)
                            noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") foi vendido para o " + Util.Util.LinkaClube(clubecomprador) + " por $" + jogadoroferta.Valor.ToString("N2");
                        else
                            noticia.Texto = Util.Util.LinkaJogador(jogador) + ", que estava sem clube, assinou contrato com o " + Util.Util.LinkaClube(clubecomprador) + ".";
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                    if (clubevendedor != null && clubevendedor.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + clubevendedor.Nome + ") foi vendido para o " + clubecomprador.Nome + " por $" + jogadoroferta.Valor.ToString("N2");
                        noticia.Usuario = clubevendedor.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    lstvendidos.Add(jogador);
                    lstposvendidos.Add(new PosicaoCompradaView() { Clube = clubecomprador, Posicao = jogador.Posicao });

                    jogadoroferta.Estagio = 3;
                    jogadorofertaRepository.SaveOrUpdate(jogadoroferta);

                    if (clubevendedor != null)
                    {
                        var escalacao = escalacaoRepository.GetAll().FirstOrDefault(x => x.Jogador != null && x.Jogador.Id == jogador.Id);
                        if (escalacao != null)
                        {
                            escalacao.Jogador = null;
                            escalacao.H = 0;
                            escalacaoRepository.SaveOrUpdate(escalacao);
                        }
                    }

                    foreach (var leilaooferta in jogadorleilaoofertaRepository.GetAll().Where(x => x.Clube.Id == clubecomprador.Id && x.JogadorLeilao.Jogador.Posicao == jogador.Posicao))
                    {
                        lstleilao.Add(leilaooferta);
                    }
                }
                else if (jogadoroferta.Pontos < 1)
                {
                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        if (clubevendedor != null)
                            noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") rejeitou sua proposta.";
                        else
                            noticia.Texto = Util.Util.LinkaJogador(jogador) + " rejeitou sua proposta.";

                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadoroferta.Estagio = 0;
                    jogadorofertaRepository.SaveOrUpdate(jogadoroferta);
                }
                else if (lstposvendidos.Where(x => x.Clube.Id == clubecomprador.Id && x.Posicao == jogador.Posicao).Count() > 0)
                {
                    jogadorofertaRepository.Delete(jogadoroferta);
                }
                else if (lstvendidos.Where(x => x.Id == jogador.Id).Count() > 0)
                {
                    if (clubecomprador.Usuario != null)
                    {
                        var clubequecomprou = lstvendidos.FirstOrDefault(x => x.Id == jogador.Id).Clube.Nome;

                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        if (clubevendedor != null)
                            noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") rejeitou sua proposta e foi vendido para o " + clubequecomprou + ".";
                        else
                            noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") rejeitou sua proposta e assinou contrato com " + clubequecomprou + ".";
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadorofertaRepository.Delete(jogadoroferta);
                }
                else if (clubevendedor != null && clubevendedor.Jogadores.Where(x => !x.Temporario).Count() < 15)
                {
                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = Util.Util.LinkaClube(clubevendedor) + " cancelou a venda de " + Util.Util.LinkaJogador(jogador) + " por estar com poucos jogadores no elenco.";
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadorofertaRepository.Delete(jogadoroferta);
                }
                else if (clubecomprador.Dinheiro < jogadoroferta.Valor)
                {
                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Sua proposta por " + Util.Util.LinkaJogador(jogador) + " foi cancelada, pois você não possui dinheiro para fechar a compra.";
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadorofertaRepository.Delete(jogadoroferta);
                }
            }

            foreach (var leilaooferta in lstleilao)
            {
                jogadorleilaoofertaRepository.Delete(leilaooferta);
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = true;
            ViewBag.VerificaTecnicos = true;
            ViewBag.AtualizarTransferencias = true;
            ViewBag.AtualizarLeiloes = false;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
        }

        [Transaction]
        public ActionResult AtualizarLeiloes()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var lstvendidos = new List<Jogador>();
            var lstposvendidos = new List<PosicaoCompradaView>();
            var lstleilao = new List<JogadorLeilaoOferta>();

            foreach (var leilao in jogadorleilaoRepository.GetAll().Where(x => x.Dia < controle.Dia && x.Estagio < 2))
            {
                if (leilao.LeilaoOfertas.Where(x => x.Pontos > 0).Count() < 1)
                {
                    foreach (var item in jogadorleilaoofertaRepository.GetAll().Where(x => x.JogadorLeilao.Id == leilao.Id))
                    {
                        jogadorleilaoofertaRepository.Delete(item);
                    }

                    if (leilao.Clube.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Texto = "Leilão por " + Util.Util.LinkaJogador(leilao.Jogador) + " foi finalizado sem ofertas.";
                        noticia.Usuario = leilao.Clube.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    leilao.Estagio = 2;
                    jogadorleilaoRepository.Delete(leilao);
                }
                else
                {
                    if (leilao.Clube.Jogadores.Where(x => !x.Temporario).Count() > 14)// && clubecomprador.Dinheiro >= jogadoroferta.JogadorLeilao.Valor && jogadoroferta.Pontos > 0)
                    {
                        var finalizado = false;

                        while (finalizado != true)
                        {
                            var comprador = leilao.LeilaoOfertas.OrderByDescending(x => x.Pontos).ThenByDescending(x => x.Salario).FirstOrDefault();

                            if (comprador != null && comprador.Clube.Dinheiro >= leilao.Valor && comprador.Pontos > 0)
                            {
                                var clubecomprador = clubeRepository.Get(comprador.Clube.Id);
                                var clubevendedor = leilao.Clube != null ? clubeRepository.Get(leilao.Clube.Id) : null;
                                var jogador = leilao.Jogador;

                                clubevendedor.Dinheiro = clubecomprador.Dinheiro + leilao.Valor;
                                clubeRepository.SaveOrUpdate(clubevendedor);

                                var historico = new JogadorHistorico();
                                historico.Ano = controle.Ano;
                                historico.Clube = clubevendedor;
                                historico.Gols = jogador.Gols.Where(x => x.Clube.Id == clubevendedor.Id).Count();
                                historico.Jogador = jogador;
                                historico.Jogos = jogador.Jogos;
                                historico.NotaMedia = jogador.NotaMedia > 0.0 ? jogador.NotaMedia : 0.00;
                                historico.Valor = leilao.Valor;
                                jogadorhistoricoRepository.SaveOrUpdate(historico);

                                jogador.Clube = clubecomprador;
                                jogador.Contrato = comprador.Contrato;
                                jogador.Salario = comprador.Salario;
                                jogador.Situacao = 1;
                                jogador.Satisfacao = 0;
                                jogador.Jogos = 0;
                                jogador.NotaTotal = 0;
                                jogador.NotaUlt = 0;
                                jogador.Treinos = 0;
                                jogador.TreinoUlt = 0;
                                jogador.TreinoTotal = 0;
                                jogadorRepository.SaveOrUpdate(jogador);

                                if (clubecomprador.Usuario != null)
                                {
                                    var noticia = new Noticia();
                                    noticia.Dia = controle.Dia;
                                    if (clubevendedor != null)
                                        noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") foi vendido para o " + Util.Util.LinkaClube(clubecomprador) + " por $" + leilao.Valor.ToString("N2");
                                    else
                                        noticia.Texto = Util.Util.LinkaJogador(jogador) + ", que estava sem clube, assinou contrato com o " + Util.Util.LinkaClube(clubecomprador) + ".";
                                    noticia.Usuario = clubecomprador.Usuario;
                                    noticiaRepository.SaveOrUpdate(noticia);
                                }
                                if (clubevendedor != null && clubevendedor.Usuario != null)
                                {
                                    var noticia = new Noticia();
                                    noticia.Dia = controle.Dia;
                                    noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + clubevendedor.Nome + ") foi vendido para o " + clubecomprador.Nome + " por $" + leilao.Valor.ToString("N2");
                                    noticia.Usuario = clubevendedor.Usuario;
                                    noticiaRepository.SaveOrUpdate(noticia);
                                }

                                lstvendidos.Add(jogador);
                                lstposvendidos.Add(new PosicaoCompradaView() { Clube = clubecomprador, Posicao = jogador.Posicao });

                                leilao.Estagio = 2;
                                leilao.Vencedor = clubecomprador;
                                jogadorleilaoRepository.SaveOrUpdate(leilao);

                                if (clubevendedor != null)
                                {
                                    var escalacao = escalacaoRepository.GetAll().FirstOrDefault(x => x.Jogador != null && x.Jogador.Id == jogador.Id);
                                    if (escalacao != null)
                                    {
                                        escalacao.Jogador = null;
                                        escalacao.H = 0;
                                        escalacaoRepository.SaveOrUpdate(escalacao);
                                    }
                                }

                                foreach (var item in jogadorleilaoofertaRepository.GetAll().Where(x => x.JogadorLeilao.Id == leilao.Id))
                                {
                                    lstleilao.Add(item);
                                }

                                finalizado = true;
                            }
                            else
                            {
                                foreach (var item in jogadorleilaoofertaRepository.GetAll().Where(x => x.JogadorLeilao.Id == leilao.Id))
                                {
                                    jogadorleilaoofertaRepository.Delete(item);
                                }

                                if (leilao.Clube.Usuario != null)
                                {
                                    var noticia = new Noticia();
                                    noticia.Texto = "Leilão por " + Util.Util.LinkaJogador(leilao.Jogador) + " foi finalizado sem ofertas.";
                                    noticia.Usuario = leilao.Clube.Usuario;
                                    noticiaRepository.SaveOrUpdate(noticia);
                                }

                                finalizado = true;
                                leilao.Estagio = 2;
                                jogadorleilaoRepository.SaveOrUpdate(leilao);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in jogadorleilaoofertaRepository.GetAll().Where(x => x.JogadorLeilao.Id == leilao.Id))
                        {
                            jogadorleilaoofertaRepository.Delete(item);
                        }

                        if (leilao.Clube.Usuario != null)
                        {
                            var noticia = new Noticia();
                            noticia.Texto = "Leilão por " + Util.Util.LinkaJogador(leilao.Jogador) + " foi cancelado pois você está no limite de jogadores.";
                            noticia.Usuario = leilao.Clube.Usuario;
                            noticiaRepository.SaveOrUpdate(noticia);
                        }

                        leilao.Estagio = 2;
                        jogadorleilaoRepository.Delete(leilao);
                    }
                }
            }

            //foreach (var jogadoroferta in jogadorleilaoofertaRepository.GetAll().Where(x => x.Estagio < 3).OrderByDescending(x => x.JogadorLeilao.Jogador.H).ThenByDescending(x => x.Pontos).ThenByDescending(x => x.Salario))
            //{
            //    var jogador = jogadoroferta.JogadorLeilao.Jogador;
            //    var clubecomprador = clubeRepository.Get(jogadoroferta.Clube.Id);
            //    var clubevendedor = jogador.Clube != null ? clubeRepository.Get(jogador.Clube.Id) : null;

            //    if (((clubevendedor != null && clubevendedor.Jogadores.Where(x => !x.Temporario).Count() > 14) || clubevendedor == null) && lstvendidos.Where(x => x.Id == jogador.Id).Count() == 0 && lstposvendidos.Where(x => x.Clube.Id == clubecomprador.Id && x.Posicao == jogador.Posicao).Count() == 0 && clubecomprador.Dinheiro >= jogadoroferta.JogadorLeilao.Valor && jogadoroferta.Pontos > 0)
            //    {
            //        clubecomprador.Dinheiro = clubecomprador.Dinheiro - jogadoroferta.JogadorLeilao.Valor;
            //        //                    clubeRepository.SaveOrUpdate(clubecomprador);

            //        if (clubevendedor != null)
            //        {
            //            clubevendedor.Dinheiro = clubecomprador.Dinheiro + jogadoroferta.JogadorLeilao.Valor;
            //            //                        clubeRepository.SaveOrUpdate(clubevendedor);

            //            var historico = new JogadorHistorico();
            //            historico.Ano = controle.Ano;
            //            historico.Clube = clubevendedor;
            //            historico.Gols = jogador.Gols.Where(x => x.Clube.Id == clubevendedor.Id).Count();
            //            historico.Jogador = jogador;
            //            historico.Jogos = jogador.Jogos;
            //            historico.NotaMedia = jogador.NotaMedia > 0.0 ? jogador.NotaMedia : 0.00;
            //            historico.Valor = jogadoroferta.JogadorLeilao.Valor;
            //            //                        jogadorhistoricoRepository.SaveOrUpdate(historico);
            //        }

            //        jogador.Clube = clubecomprador;
            //        jogador.Contrato = jogadoroferta.Contrato;
            //        jogador.Salario = jogadoroferta.Salario;
            //        jogador.Situacao = 1;
            //        jogador.Satisfacao = 0;
            //        jogador.Jogos = 0;
            //        jogador.NotaTotal = 0;
            //        jogador.NotaUlt = 0;
            //        jogador.Treinos = 0;
            //        jogador.TreinoUlt = 0;
            //        jogador.TreinoTotal = 0;
            //        //                    jogadorRepository.SaveOrUpdate(jogador);

            //        if (clubecomprador.Usuario != null)
            //        {
            //            var noticia = new Noticia();
            //            noticia.Dia = controle.Dia;
            //            if (clubevendedor != null)
            //                noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") foi vendido para o " + Util.Util.LinkaClube(clubecomprador) + " por $" + jogadoroferta.JogadorLeilao.Valor.ToString("N2");
            //            else
            //                noticia.Texto = Util.Util.LinkaJogador(jogador) + ", que estava sem clube, assinou contrato com o " + Util.Util.LinkaClube(clubecomprador) + ".";
            //            noticia.Usuario = clubecomprador.Usuario;
            //            noticiaRepository.SaveOrUpdate(noticia);
            //        }
            //        if (clubevendedor != null && clubevendedor.Usuario != null)
            //        {
            //            var noticia = new Noticia();
            //            noticia.Dia = controle.Dia;
            //            noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + clubevendedor.Nome + ") foi vendido para o " + clubecomprador.Nome + " por $" + jogadoroferta.JogadorLeilao.Valor.ToString("N2");
            //            noticia.Usuario = clubevendedor.Usuario;
            //            noticiaRepository.SaveOrUpdate(noticia);
            //        }

            //        lstvendidos.Add(jogador);
            //        lstposvendidos.Add(new PosicaoCompradaView() { Clube = clubecomprador, Posicao = jogador.Posicao });

            //        jogadoroferta.Estagio = 3;
            //        //                    jogadorleilaoofertaRepository.SaveOrUpdate(jogadoroferta);

            //        if (clubevendedor != null)
            //        {
            //            var escalacao = escalacaoRepository.GetAll().FirstOrDefault(x => x.Jogador != null && x.Jogador.Id == jogador.Id);
            //            if (escalacao != null)
            //            {
            //                escalacao.Jogador = null;
            //                escalacao.H = 0;
            //                //                            escalacaoRepository.SaveOrUpdate(escalacao);
            //            }
            //        }
            //    }
            //    else if (jogadoroferta.Pontos < 1)
            //    {
            //        if (clubecomprador.Usuario != null)
            //        {
            //            var noticia = new Noticia();
            //            noticia.Dia = controle.Dia;
            //            if (clubevendedor != null)
            //                noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") rejeitou sua proposta.";
            //            else
            //                noticia.Texto = Util.Util.LinkaJogador(jogador) + " rejeitou sua proposta.";

            //            noticia.Usuario = clubecomprador.Usuario;
            //            noticiaRepository.SaveOrUpdate(noticia);
            //        }

            //        jogadoroferta.Estagio = 0;
            //        jogadorleilaoofertaRepository.Delete(jogadoroferta);
            //    }
            //    else if (lstposvendidos.Where(x => x.Clube.Id == clubecomprador.Id && x.Posicao == jogador.Posicao).Count() > 0)
            //    {
            //        jogadorleilaoofertaRepository.Delete(jogadoroferta);
            //    }
            //    else if (lstvendidos.Where(x => x.Id == jogador.Id).Count() > 0)
            //    {
            //        if (clubecomprador.Usuario != null)
            //        {
            //            var clubequecomprou = lstvendidos.FirstOrDefault(x => x.Id == jogador.Id).Clube.Nome;

            //            var noticia = new Noticia();
            //            noticia.Dia = controle.Dia;
            //            if (clubevendedor != null)
            //                noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") rejeitou sua proposta e foi vendido para o " + clubequecomprou + ".";
            //            else
            //                noticia.Texto = Util.Util.LinkaJogador(jogador) + " (" + Util.Util.LinkaClube(clubevendedor) + ") rejeitou sua proposta e assinou contrato com " + clubequecomprou + ".";
            //            noticia.Usuario = clubecomprador.Usuario;
            //            noticiaRepository.SaveOrUpdate(noticia);
            //        }

            //        jogadorleilaoofertaRepository.Delete(jogadoroferta);
            //    }
            //    else if (clubevendedor != null && clubevendedor.Jogadores.Where(x => !x.Temporario).Count() < 15)
            //    {
            //        if (clubecomprador.Usuario != null)
            //        {
            //            var noticia = new Noticia();
            //            noticia.Dia = controle.Dia;
            //            noticia.Texto = Util.Util.LinkaClube(clubevendedor) + " cancelou o leilão de " + Util.Util.LinkaJogador(jogador) + " por estar com poucos jogadores no elenco.";
            //            noticia.Usuario = clubecomprador.Usuario;
            //            noticiaRepository.SaveOrUpdate(noticia);
            //        }

            //        jogadorleilaoofertaRepository.Delete(jogadoroferta);
            //    }
            //    else if (clubecomprador.Dinheiro < jogadoroferta.JogadorLeilao.Valor)
            //    {
            //        if (clubecomprador.Usuario != null)
            //        {
            //            var noticia = new Noticia();
            //            noticia.Dia = controle.Dia;
            //            noticia.Texto = "Sua proposta por " + Util.Util.LinkaJogador(jogador) + " foi cancelada, pois você não possui dinheiro para fechar a compra.";
            //            noticia.Usuario = clubecomprador.Usuario;
            //            noticiaRepository.SaveOrUpdate(noticia);
            //        }

            //        jogadorleilaoofertaRepository.Delete(jogadoroferta);
            //    }
            //}

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = true;
            ViewBag.VerificaTecnicos = true;
            ViewBag.AtualizarTransferencias = true;
            ViewBag.AtualizarLeiloes = true;
            ViewBag.VariarJogador = 0;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            //return View("Index");
            return View("Rodando");
        }

        [Transaction]
        public ActionResult VariarJogador(int fase)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var lstJogador = jogadorRepository.GetAll().Where(x => !x.Temporario);
            var skip = 0;
            var take = lstJogador.Count() / 2;
            if (fase > 0)
            {
                skip = lstJogador.Count() / 2;
                take = lstJogador.Count() - skip;
            }

            var rnd = new Random();
            foreach (var jogador in lstJogador.Skip(skip).Take(take))
            {
                var escalacao = escalacaoRepository.GetAll().FirstOrDefault(x => x.Jogador != null && x.Jogador.Id == jogador.Id);

                if (escalacao == null)
                {
                    var variavel = rnd.Next(0, 1);
                    jogador.H = jogador.H + (variavel);

                    if (jogador.H > (jogador.HF + 10))
                        jogador.H = jogador.HF + 10;
                    else if (jogador.H < (jogador.HF - 10))
                        jogador.H = jogador.HF - 10;

                    if (jogador.H > 99)
                        jogador.H = 99;
                    else if (jogador.H < 1)
                        jogador.H = 1;
                }

                if (jogador.Lesionado == 0 && jogador.Clube != null)
                {
                    var chancelesionar = 2;

                    if (jogador.Condicao < 30)
                        chancelesionar = 80;
                    else if (jogador.Condicao < 40)
                        chancelesionar = 20;
                    else if (jogador.Condicao < 50)
                        chancelesionar = 10;
                    else if (jogador.Condicao < 60)
                        chancelesionar = 5;
                    else if (jogador.Condicao < 80)
                        chancelesionar = 1;

                    if (rnd.Next(1, 101) > chancelesionar)
                    {
                        //var clubejogadores = jogador.Clube.Jogadores.Where(x => !x.Temporario);

                        //var hmediotime = clubejogadores.Sum(x => x.H) / clubejogadores.Count();

                        //var probjogarbem = 50 + (jogador.H - hmediotime);
                        //probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                        //double nota = 0;
                        //var rndnota = rnd.Next(1, 101);
                        //var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                        //var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                        //var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                        //var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                        //var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                        //var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                        //var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                        //if (rnd.Next(1, 101) <= probjogarbem)
                        //{
                        //    if (rndnota < nota1)
                        //        nota = 5.5;
                        //    else if (rndnota < (nota1 + nota2))
                        //        nota = 6.5;
                        //    else if (rndnota < (nota1 + nota2 + nota3))
                        //        nota = 7.0;
                        //    else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                        //        nota = 7.5;
                        //    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                        //        nota = 8.0;
                        //    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                        //        nota = 8.5;
                        //    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                        //        nota = 9.0;
                        //    else
                        //        nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                        //}
                        //else
                        //{
                        //    if (rndnota < nota1)
                        //        nota = 5.0;
                        //    else if (rndnota < (nota1 + nota2))
                        //        nota = 4.5;
                        //    else if (rndnota < (nota1 + nota2 + nota3))
                        //        nota = 4.0;
                        //    else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                        //        nota = 3.5;
                        //    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                        //        nota = 2.5;
                        //    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                        //        nota = 2.0;
                        //    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                        //        nota = 1.5;
                        //    else
                        //        nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                        //}

                        //jogador.Treinos = jogador.Treinos + 1;
                        //jogador.TreinoTotal = jogador.TreinoTotal + Convert.ToDecimal(nota);
                        //jogador.TreinoUlt = Convert.ToDecimal(nota);

                        if (jogador.Clube.Usuario == null)
                            jogador.Condicao = 100;

                        if (jogador.Condicao < 90)
                            jogador.Condicao = jogador.Condicao + 10;
                        else
                            jogador.Condicao = 100;
                    }
                    else
                    {
                        var tempo = rnd.Next(1, 11);

                        if (jogador.Clube.Usuario != null)
                        {
                            var usuario = jogador.Clube.Usuario;
                            if (usuario.Staffs.Where(x => x.Tipo == 2).Count() > 0)
                            {
                                var medico = usuario.Staffs.Where(x => x.Tipo == 2).FirstOrDefault();
                                if (rnd.Next(1, 101) <= medico.H)
                                    tempo = tempo / 2;
                            }
                        }
                        else
                            tempo = tempo / 2;

                        if (tempo < 1)
                            tempo = 1;

                        jogador.Lesionado = tempo;
                        jogadorRepository.SaveOrUpdate(jogador);

                        if (escalacao != null)
                        {
                            escalacao.Jogador = null;
                            escalacao.H = 0;
                            escalacaoRepository.SaveOrUpdate(escalacao);
                        }

                        if (jogador.Clube.Usuario != null)
                        {
                            var noticia = new Noticia();
                            noticia.Dia = controle.Dia;
                            noticia.Texto = Util.Util.LinkaJogador(jogador) + " se lesionou por " + jogador.Lesionado + " dia(s)";
                            noticia.Usuario = jogador.Clube.Usuario;
                            noticiaRepository.SaveOrUpdate(noticia);
                        }
                    }
                }
                else if (jogador.Lesionado > 0)
                {
                    jogador.Lesionado = jogador.Lesionado - 1;
                    jogador.TreinoUlt = 0;
                    jogador.Condicao = 90;

                    if (jogador.Lesionado == 0 && jogador.Clube != null && jogador.Clube.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = Util.Util.LinkaJogador(jogador) + " está recuperado da lesão e disponível para jogar.";
                        noticia.Usuario = jogador.Clube.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                }

                jogadorRepository.SaveOrUpdate(jogador);
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = false;
            ViewBag.UpdateFinancas = true;
            ViewBag.VerificaTecnicos = true;
            ViewBag.AtualizarTransferencias = true;
            ViewBag.AtualizarLeiloes = true;
            if (fase == 0)
                ViewBag.VariarJogador = 1;
            else
                ViewBag.VariarJogador = 2;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
        }

        [Transaction]
        public ActionResult CriarJogadoresTemporarios()
        {
            foreach (var clube in clubeRepository.GetAll())
            {
                var nomes = nomeRepository.GetAll();
                var rnd = new Random();

                rnd.Next(0, clube.Id);

                for (int pos = 1; pos < 8; pos++)
                {
                    var min = 0;

                    if (pos == 1 || pos == 2 || pos == 4)
                        min = 1;
                    else if (pos == 3 || pos == 5 || pos == 6 || pos == 7)
                        min = 2;

                    if (clube.Jogadores.Where(x => x.Posicao == pos && !x.Temporario && x.Lesionado == 0).Count() < min)
                    {
                        var qnttemp = min - clube.Jogadores.Where(x => x.Posicao == pos && x.Lesionado == 0).Count();

                        for (int novos = 1; novos <= qnttemp; novos++)
                        {
                            var jogador = new Jogador();
                            jogador.Clube = clube;
                            jogador.Posicao = pos;
                            jogador.Temporario = true;
                            jogador.H = 1;
                            jogador.HF = 1;

                            var objnome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                            if (!objnome.Comum) { objnome = nomes.ElementAt(rnd.Next(0, nomes.Count())); }

                            jogador.Nome = objnome.NomeJogador.ToUpper();
                            jogadorRepository.SaveOrUpdate(jogador);
                        }
                    }
                    else if (clube.Jogadores.Where(x => x.Posicao == pos && !x.Temporario).Count() >= min)
                    {
                        foreach (var jog in clube.Jogadores.Where(x => x.Posicao == pos && x.Temporario))
                        {
                            jog.Clube = null;
                            jogadorRepository.SaveOrUpdate(jog);
                        }
                    }
                }
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = true;
            ViewBag.UpdateFinancas = true;
            ViewBag.VerificaTecnicos = true;
            ViewBag.AtualizarTransferencias = true;
            ViewBag.AtualizarLeiloes = true;
            ViewBag.VariarJogador = 2;
            ViewBag.ZeraStaffConsultas = false;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
            //return View("Index");
        }

        [Transaction]
        public ActionResult ZeraStaffConsultas()
        {
            foreach (var staff in staffRepository.GetAll().Where(x => x.Tipo == 1 && x.Consultas < (x.H / 10)))
            {
                staff.Consultas = staff.H / 10;
                staffRepository.SaveOrUpdate(staff);
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = true;
            ViewBag.UpdateFinancas = true;
            ViewBag.VerificaTecnicos = true;
            ViewBag.AtualizarTransferencias = true;
            ViewBag.AtualizarLeiloes = true;
            ViewBag.VariarJogador = 2;
            ViewBag.ZeraStaffConsultas = true;
            ViewBag.GerarTesteJogador = false;

            return View("Rodando");
        }

        [Transaction]
        public ActionResult GerarTesteJogador()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var rnd = new Random();
            foreach (var teste in jogadortesteRepository.GetAll().Where(x => x.Dia < controle.Dia))
            {
                var jogador = teste.Jogador;

                var clubejogadores = teste.Clube.Jogadores.Where(x => !x.Temporario);

                var hmediotime = clubejogadores.Sum(x => x.H) / clubejogadores.Count();

                var probjogarbem = 50 + (jogador.H - hmediotime);
                probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                double nota = 0;
                var rndnota = rnd.Next(1, 101);
                var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                if (rnd.Next(1, 101) <= probjogarbem)
                {
                    if (rndnota < nota1)
                        nota = 5.5;
                    else if (rndnota < (nota1 + nota2))
                        nota = 6.5;
                    else if (rndnota < (nota1 + nota2 + nota3))
                        nota = 7.0;
                    else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                        nota = 7.5;
                    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                        nota = 8.0;
                    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                        nota = 8.5;
                    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                        nota = 9.0;
                    else
                        nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                }
                else
                {
                    if (rndnota < nota1)
                        nota = 5.0;
                    else if (rndnota < (nota1 + nota2))
                        nota = 4.5;
                    else if (rndnota < (nota1 + nota2 + nota3))
                        nota = 4.0;
                    else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                        nota = 3.5;
                    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                        nota = 2.5;
                    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                        nota = 2.0;
                    else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                        nota = 1.5;
                    else
                        nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                }

                if (teste.Clube.Usuario != null)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = Util.Util.LinkaJogador(jogador) + " foi testado e participou do treino com seus jogadores. Ele foi avaliado com uma nota <b>" + nota + "</b>.";
                    noticia.Usuario = teste.Clube.Usuario;
                    noticiaRepository.SaveOrUpdate(noticia);
                }

                jogadortesteRepository.Delete(teste);
            }

            ViewBag.AtualizarDataDia = true;
            ViewBag.EscalaTimes = true;
            ViewBag.RodaPartida = true;
            ViewBag.AtualizaTabela = true;
            ViewBag.GerarTaca = true;
            ViewBag.ZerarDelayUsuario = true;
            ViewBag.CriarJogadoresTemporarios = true;
            ViewBag.UpdateFinancas = true;
            ViewBag.VerificaTecnicos = true;
            ViewBag.AtualizarTransferencias = true;
            ViewBag.AtualizarLeiloes = true;
            ViewBag.VariarJogador = 2;
            ViewBag.ZeraStaffConsultas = true;
            ViewBag.GerarTesteJogador = true;

            return View("Rodando");
        }

        #endregion

        #region MudarAno

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

                var noticia = new Noticia();
                noticia.Dia = controle.Dia;
                noticia.Texto = "Parabéns! Você conquistou a TAÇA!";
                noticiaRepository.SaveOrUpdate(noticia);
            }

            historico.Ano = controle.Ano;
            historico.Taca = true;
            historico.Campeao = partidafinaltaca.Vencedor;
            historico.Vice = partidafinaltaca.Vencedor == partidafinaltaca.Clube1 ? partidafinaltaca.Clube2 : partidafinaltaca.Clube1;
            var artilheiro = artilheiroRepository.GetAll().OrderByDescending(x => x.Taca).FirstOrDefault();
            historico.Artilheiro = artilheiro.Jogador;
            historico.ArtilheiroClube = artilheiro.Jogador.Clube;
            historico.Gols = artilheiro.Taca;
            historicoRepository.SaveOrUpdate(historico);

            var clube = clubeRepository.Get(partidafinaltaca.Vencedor.Id);
            clube.Socios = clube.Socios + 4000;
            clubeRepository.SaveOrUpdate(clube);

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("ZerarTransferencias", "Engine");
        }

        [Transaction]
        public ActionResult ZerarTransferencias()
        {
            foreach (var jogadoroferta in jogadorofertaRepository.GetAll().Where(x => x.Estagio == 0 || x.Estagio == 3))
            {
                jogadorofertaRepository.Delete(jogadoroferta);
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("ZerarAnoStaff", "Engine");
        }

        [Transaction]
        public ActionResult ZerarPatrocinio()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            foreach (var patrociniorecusa in patrociniorecusaRepository.GetAll())
            {
                patrociniorecusaRepository.Delete(patrociniorecusa);
            }

            foreach (var patrocinioclube in patrocinioclubeRepository.GetAll())
            {
                patrocinioclube.Contrato = (patrocinioclube.Contrato - 1) >= 0 ? patrocinioclube.Contrato - 1 : 0;

                if (patrocinioclube.Contrato == 0)
                {
                    if (patrocinioclube.Clube.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Seu contrato com " + Util.Util.LinkaPatrocinio(patrocinioclube.Patrocinio) + " terminou.";
                        noticia.Usuario = patrocinioclube.Clube.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    patrocinioclubeRepository.Delete(patrocinioclube);
                }
                else
                    patrocinioclubeRepository.SaveOrUpdate(patrocinioclube);
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("ZerarAnoStaff", "Engine");
        }

        [Transaction]
        public ActionResult ZerarAnoStaff()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var staff in staffRepository.GetAll().Where(x => x.Usuario != null))
            {
                staff.Contrato = (staff.Contrato - 1) >= 0 ? staff.Contrato - 1 : 0;

                if (staff.Contrato == 0)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Seu contrato com o " + Util.Util.RetornaStaffTipo(staff.Tipo) + " " + Util.Util.LinkaStaff(staff) + " terminou. Ele deixou sua comissão técnica.";
                    noticia.Usuario = staff.Usuario;
                    noticiaRepository.SaveOrUpdate(noticia);

                    staff.Usuario = null;
                    staff.Salario = 0;
                }

                staffRepository.SaveOrUpdate(staff);
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("ClassificadosTaca", "Engine");
        }

        [Transaction]
        public ActionResult ClassificadosTaca()
        {
            var itaca = 1;
            var controle = controleRepository.GetAll().FirstOrDefault();

            foreach (var divisaotabela in divisaotabelaRepository.GetAll().OrderBy(x => x.Divisao.Numero).ThenBy(x => x.Posicao))
            {
                var clube = clubeRepository.Get(divisaotabela.Clube.Id);
                clube.Taca = true;

                clubeRepository.SaveOrUpdate(clube);

                if (clube.Usuario != null)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Devido sua colocação você foi classificado para participar da TAÇA.";
                    noticia.Usuario = clube.Usuario;
                    noticiaRepository.SaveOrUpdate(noticia);
                }

                itaca++;
                if (itaca > 32)
                    break;
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("AlterarDivisaoTimes", "Engine");
        }

        [Transaction]
        public ActionResult AlterarDivisaoTimes()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
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

                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Parabéns! Você foi o CAMPEÃO da " + divisao.Nome + "!";
                    noticiaRepository.SaveOrUpdate(noticia);
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

                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Parabéns! Você foi o vice-campeão da " + divisao.Nome + "!";
                    noticiaRepository.SaveOrUpdate(noticia);
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
                var historico = new Historico();
                historico.Usuario = clube1.Usuario != null ? clube1.Usuario : null;
                historico.Ano = controle.Ano;
                historico.Divisao = divisao;
                historico.Campeao = clube1;
                historico.Vice = clube2;
                var artilheiro = artilheiroRepository.GetAll().Where(x => x.Clube.Divisao.Id == divisao.Id).OrderByDescending(x => x.Divisao).FirstOrDefault();
                historico.Artilheiro = artilheiro.Jogador;
                historico.ArtilheiroClube = artilheiro.Jogador.Clube;
                historico.Gols = artilheiro.Divisao;
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

                    if (clube1.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Você foi promovido para a " + promodivisao.Nome + " devido ao título!";
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                    if (clube2.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Você foi promovido para a " + promodivisao.Nome + " devido ao vice-campeonato!";
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                }

                if (divisao.Numero < ultdivisao)
                {
                    var rebaixadivisao = divisaoRepository.GetAll().Where(x => x.Numero == (divisao.Numero + 1)).FirstOrDefault();

                    clube11.Divisao = rebaixadivisao;
                    clube12.Divisao = rebaixadivisao;

                    if (clube11.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Você foi rebaixado para a " + rebaixadivisao.Nome + " devido a sua posição final!";
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                    if (clube12.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Você foi promovido para a " + rebaixadivisao.Nome + " devido a sua posição final!";
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                }

                clube1.Socios = clube1.Socios + 4000 / divisao.Numero;
                clube2.Socios = clube2.Socios + 2000 / divisao.Numero;
                clube11.Socios = clube11.Socios - 1000 / divisao.Numero;
                clube12.Socios = clube12.Socios - 1000 / divisao.Numero;

                clubeRepository.SaveOrUpdate(clube1);
                clubeRepository.SaveOrUpdate(clube2);
                clubeRepository.SaveOrUpdate(clube11);
                clubeRepository.SaveOrUpdate(clube12);
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("ZeraAnoJogadores", "Engine");
        }

        [Transaction]
        public ActionResult ZeraAnoJogadores()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var jog in jogadorRepository.GetAll().Where(x => !x.Temporario && x.Clube != null))
            {
                var historico = new JogadorHistorico();
                historico.Ano = controle.Ano;
                historico.Clube = jog.Clube;
                historico.Jogador = jog;
                historico.NotaMedia = jog.NotaMedia > 0.0 ? jog.NotaMedia : 0.00;
                historico.Jogos = jog.Jogos;
                historico.Gols = jog.Gols.Where(x => x.Clube.Id == jog.Clube.Id).Count();
                jogadorhistoricoRepository.SaveOrUpdate(historico);

                jog.Jogos = 0;
                jog.NotaTotal = 0;
                jog.NotaUlt = 0;
                jog.Treinos = 0;
                jog.TreinoTotal = 0;
                jog.TreinoUlt = 0;
                jog.Contrato = (jog.Contrato - 1) >= 0 ? jog.Contrato - 1 : 0;

                if (jog.Contrato == 0)
                {
                    if (jog.Clube.Jogadores.Count() > 14)
                    {
                        if (jog.Clube.Usuario != null)
                        {
                            var noticia = new Noticia();
                            noticia.Dia = controle.Dia;
                            noticia.Texto = Util.Util.LinkaJogador(jog) + " encerrou seu contrato e deixou o clube.";
                            noticia.Usuario = jog.Clube.Usuario;
                        }

                        jog.Clube = null;
                        jog.Jogos = 0;
                        jog.Salario = 0;
                    }
                    else
                    {
                        if (jog.Clube.Usuario != null)
                        {
                            var noticia = new Noticia();
                            noticia.Dia = controle.Dia;
                            noticia.Texto = Util.Util.LinkaJogador(jog) + " teve seu contrato prorrogado por 1 ano com aumento de 20%, pois você não pode ter menos que 14 jogadores.";
                            noticia.Usuario = jog.Clube.Usuario;
                        }

                        jog.Salario = (jog.Salario / 100) * 120;
                        jog.Contrato = 1;
                    }
                }


                jogadorRepository.SaveOrUpdate(jog);
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("ZerarCampeonato", "Engine");
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
            //return RedirectToAction("GerarCampeonato", "Engine");
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
            //return RedirectToAction("GerarTacaAno", "Engine");
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
            //return RedirectToAction("ZerarJogadoresTemporarios", "Engine");
        }

        [Transaction]
        public ActionResult ZerarJogadoresTemporarios()
        {
            foreach (var jogador in jogadorRepository.GetAll().Where(x => x.Temporario && x.Clube == null))
                jogadorRepository.Delete(jogador);

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("AtualizarDataAno", "Engine");
        }

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
            //return RedirectToAction("IniciarAno", "Engine");
        }

        [Transaction]
        public ActionResult IniciarAno()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var clube in clubeRepository.GetAll())
            {
                var valor = 8000000 / clube.Divisao.Numero;

                if (clube.Usuario != null)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Você recebeu $" + valor.ToString("N2") + " pelos direitos televisivos da " + clube.Divisao.Nome + ".";
                    noticia.Usuario = clube.Usuario;
                    noticiaRepository.SaveOrUpdate(noticia);
                }

                clube.Dinheiro = clube.Dinheiro + valor;
                clubeRepository.SaveOrUpdate(clube);
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region GerarJogador

        [Transaction]
        public ActionResult GerarJogador(int? id)
        {
            Clube clube = null;
            int? divisao = null;

            if (id.HasValue)
            {
                clube = clubeRepository.Get(id.Value);
                divisao = clube.Divisao.Numero;
            }
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

                if (g > 0) { jogador.Posicao = 1; g--; }
                else if (ld > 0) { jogador.Posicao = 2; ld--; }
                else if (z > 0) { jogador.Posicao = 3; z--; }
                else if (le > 0) { jogador.Posicao = 4; le--; }
                else if (v > 0) { jogador.Posicao = 5; v--; }
                else if (mo > 0) { jogador.Posicao = 6; mo--; }
                else if (a > 0) { jogador.Posicao = 7; a--; }

                var objnome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                if (!objnome.Comum) { objnome = nomes.ElementAt(rnd.Next(0, nomes.Count())); }

                jogador.Nome = objnome.NomeJogador.ToUpper();

                if (clube != null)
                {
                    jogador.Clube = clube;

                    var h = Convert.ToInt32((60 / divisao) / 5) * 5;
                    h = h + (5 * rnd.Next(1, 5)) - 10;
                    jogador.HF = h > 0 ? h : 1;

                    jogador.H = jogador.HF;
                    jogador.Salario = ((jogador.HF / 2) / divisao.Value) * 1000;
                    jogador.Contrato = 2;
                }
                else
                {
                    var prob1 = rnd.Next(1, 101);
                    if (prob1 >= 95)
                        jogador.H = rnd.Next(70, 91);
                    else if (prob1 >= 65)
                        jogador.H = rnd.Next(40, 70);
                    else if (prob1 >= 30)
                        jogador.H = rnd.Next(20, 40);
                    else
                        jogador.H = rnd.Next(1, 20);
                }
                jogadorRepository.SaveOrUpdate(jogador);
            }

            return RedirectToAction("DetalheClube", "Adm", new { id = id });
        }

        #endregion

        #region GerarStaff

        [Transaction]
        public ActionResult GerarStaff()
        {
            var nomes = nomeRepository.GetAll();
            var sobrenomes = sobrenomeRepository.GetAll();
            var rnd = new Random();

            for (int i = 0; i < 25; i++)
            {
                var staff = new Staff();

                var nome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                var sobrenome = sobrenomes.ElementAt(rnd.Next(0, sobrenomes.Count()));

                staff.Nome = nome.NomeJogador.ToUpper() + " " + sobrenome.SobrenomeJogador.ToUpper();
                staff.Tipo = 1;
                staff.H = rnd.Next(1, 101);

                staffRepository.SaveOrUpdate(staff);
            }

            for (int i = 0; i < 25; i++)
            {
                var staff = new Staff();

                var nome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                var sobrenome = sobrenomes.ElementAt(rnd.Next(0, sobrenomes.Count()));

                staff.Nome = nome.NomeJogador.ToUpper() + " " + sobrenome.SobrenomeJogador.ToUpper();

                staff.H = rnd.Next(1, 101);
                staff.Tipo = 2;
                staffRepository.SaveOrUpdate(staff);
            }

            return RedirectToAction("GridStaff", "Adm");
        }

        #endregion

        #region GerarPrimeiroEscalacao

        [Transaction]
        public ActionResult GerarPrimeiroEscalacao()
        {
            foreach (var clube in clubeRepository.GetAll().Where(x => x.Escalacao.Count() == 0))
            {
                for (int i = 0; i < 11; i++)
                {
                    var escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacaoRepository.SaveOrUpdate(escalacao);
                }
            }

            return RedirectToAction("Index", "Engine");
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

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////ALTERAR ABAIXO
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////ALTERAR ABAIXO
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////ALTERAR ABAIXO

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

            var Prob1 = 55;
            var Prob2 = 45;
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
                else { clube1.ReputacaoAI = clube1.ReputacaoAI + 4 > 50 ? 50 : clube1.ReputacaoAI + 4; }

                if (clube2.Usuario != null)
                {
                    var usuario = clube2.Usuario;
                    usuario.Reputacao = usuario.Reputacao - 6 < 0 ? 0 : usuario.Reputacao - 6;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                else { clube2.ReputacaoAI = clube2.ReputacaoAI - 6 < 0 ? 0 : clube2.ReputacaoAI - 6; }

                decimal notaclube1 = 0;
                decimal notaclube2 = 0;

                //notas CLUBE 1
                foreach (var escalacao in clube1.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa2);

                        if ((jogador.H - Defesa2) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem + 15;

                    }
                    else
                    {
                        probjogarbem = 50 + (jogador.H - Ataque2);

                        if ((jogador.H - Ataque2) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem + 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube1 = notaclube1 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                //notas CLUBE 2
                foreach (var escalacao in clube2.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa1);

                        if ((jogador.H - Defesa1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else
                            probjogarbem = probjogarbem - 15;

                    }
                    else
                    {
                        probjogarbem = 50 + (jogador.H - Ataque1);

                        if ((jogador.H - Ataque1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube2 = notaclube2 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                notaclube1 = notaclube1 / 11;
                notaclube2 = notaclube2 / 11;

                var dif7 = 0;
                var dif6 = 0;
                var dif5 = 0;
                var dif4 = 0;
                var dif3 = 0;
                var dif2 = 0;

                if (notaclube1 >= notaclube2)
                {
                    if ((notaclube1 - notaclube2) > 3)
                    {
                        dif7 = 2; dif6 = 5; dif5 = 8; dif4 = 15; dif3 = 35; dif2 = 30; //dif1 = 5
                    }
                    else if ((notaclube1 - notaclube2) > 2)
                    {
                        dif6 = 2; dif5 = 5; dif4 = 8; dif3 = 30; dif2 = 40; //dif1 = 15
                    }
                    else if ((notaclube1 - notaclube2) > 1)
                    {
                        dif5 = 2; dif4 = 8; dif3 = 10; dif2 = 50; //dif1 = 30
                    }
                    else
                    {
                        dif4 = 2; dif3 = 8; dif2 = 35; //dif1 = 55
                    }
                }
                else
                {
                    if ((notaclube2 - notaclube1) > 3)
                    {
                        dif2 = 2; //dif1 = 98
                    }
                    else if ((notaclube2 - notaclube1) > 2)
                    {
                        dif2 = 10; //dif1 = 90
                    }
                    else if ((notaclube2 - notaclube1) > 1)
                    {
                        dif4 = 2; dif3 = 8; dif2 = 20; //dif1 = 70
                    }
                    else
                    {
                        dif4 = 2; dif3 = 8; dif2 = 40; //dif1 = 50
                    }
                }

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
                    gol2 = rnd.Next(0, 2);
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
                else
                {
                    clube1.ReputacaoAI = clube1.ReputacaoAI - 3 < 0 ? 0 : clube1.ReputacaoAI - 3;
                }

                decimal notaclube1 = 0;
                decimal notaclube2 = 0;

                //notas CLUBE 1
                foreach (var escalacao in clube1.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa2);

                        if ((jogador.H - Defesa2) > 20)
                            probjogarbem = probjogarbem - 40;
                        else if ((jogador.H - Defesa2) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem - 15;

                    }
                    else
                    {
                        probjogarbem = 50 + (jogador.H - Ataque2);

                        if ((jogador.H - Ataque2) > 20)
                            probjogarbem = probjogarbem - 40;
                        else if ((jogador.H - Ataque2) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube1 = notaclube1 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                //notas CLUBE 2
                foreach (var escalacao in clube2.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa1);

                        if ((jogador.H - Defesa1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else if ((jogador.H - Defesa1) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem - 15;

                    }
                    else
                    {
                        if ((jogador.H - Ataque1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else if ((jogador.H - Ataque1) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube2 = notaclube2 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
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
                else
                {
                    clube1.ReputacaoAI = clube1.ReputacaoAI - 8 < 0 ? 0 : clube1.ReputacaoAI - 8;
                }

                if (clube2.Usuario != null)
                {
                    var usuario = clube2.Usuario;

                    usuario.Reputacao = usuario.Reputacao + 6 > 50 ? 50 : usuario.Reputacao + 6;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube2.ReputacaoAI = clube2.ReputacaoAI + 6 > 50 ? 50 : clube2.ReputacaoAI + 6;
                }

                decimal notaclube1 = 0;
                decimal notaclube2 = 0;

                //notas CLUBE 1
                foreach (var escalacao in clube1.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa2);

                        if ((jogador.H - Defesa2) > 20)
                            probjogarbem = probjogarbem - 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }
                    else
                    {
                        probjogarbem = 50 - (jogador.H + Ataque2);

                        if ((jogador.H - Ataque1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube1 = notaclube1 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                //notas CLUBE 2
                foreach (var escalacao in clube2.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa1);

                        if ((jogador.H - Defesa1) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem + 15;

                    }
                    else
                    {
                        probjogarbem = 50 + (jogador.H - Ataque1);

                        if ((jogador.H - Ataque1) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem + 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {


                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube2 = notaclube2 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                notaclube1 = notaclube1 / 11;
                notaclube2 = notaclube2 / 11;

                var dif7 = 0;
                var dif6 = 0;
                var dif5 = 0;
                var dif4 = 0;
                var dif3 = 0;
                var dif2 = 0;

                if (notaclube2 >= notaclube1)
                {
                    if ((notaclube2 - notaclube1) > 3)
                    {
                        dif7 = 2; dif6 = 5; dif5 = 8; dif4 = 15; dif3 = 35; dif2 = 30; //dif1 = 5
                    }
                    else if ((notaclube2 - notaclube1) > 2)
                    {
                        dif6 = 2; dif5 = 5; dif4 = 8; dif3 = 30; dif2 = 40; //dif1 = 15
                    }
                    else if ((notaclube2 - notaclube1) > 1)
                    {
                        dif5 = 2; dif4 = 8; dif3 = 10; dif2 = 50; //dif1 = 30
                    }
                    else
                    {
                        dif4 = 2; dif3 = 8; dif2 = 35; //dif1 = 55
                    }
                }
                else
                {
                    if ((notaclube1 - notaclube2) > 3)
                    {
                        dif2 = 2; //dif1 = 98
                    }
                    else if ((notaclube1 - notaclube2) > 2)
                    {
                        dif2 = 10; //dif1 = 90
                    }
                    else if ((notaclube1 - notaclube2) > 1)
                    {
                        dif4 = 2; dif3 = 8; dif2 = 20; //dif1 = 70
                    }
                    else
                    {
                        dif4 = 2; dif3 = 8; dif2 = 40; //dif1 = 50
                    }
                }

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
                    gol1 = rnd.Next(0, 2);
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
                var totalgols = clube1.Escalacao.OrderByDescending(x => x.Jogador.NotaUlt).Take(5).Sum(x => x.HGol) + 1;
                for (int i = 1; i <= gol1; i++)
                {
                    var gol = new Gol();
                    gol.Clube = clube1;
                    gol.Minuto = rnd.Next(1, 94);
                    gol.Partida = partida;
                    var contagem = 0;
                    var goleador = rnd.Next(1, totalgols);
                    foreach (var jog in clube1.Escalacao.OrderByDescending(x => x.Jogador.NotaUlt).ThenByDescending(x => x.Posicao).Take(5))
                    {
                        contagem = contagem + jog.HGol;
                        if (goleador <= contagem)
                        {
                            gol.Jogador = jog.Jogador;
                            break;
                        }
                    }
                    lstGols.Add(gol); /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR
                    //golRepository.SaveOrUpdate(gol);
                }
            }

            rnd = new Random();
            rnd.Next(1, 100);

            var listgol2 = new List<Gol>();
            if (gol2 > 0)
            {
                var totalgols = clube2.Escalacao.OrderByDescending(x => x.Jogador.NotaUlt).Take(5).Sum(x => x.HGol) + 1;
                for (int i = 1; i <= gol2; i++)
                {
                    var gol = new Gol();
                    gol.Clube = clube2;
                    gol.Minuto = rnd.Next(1, 94);
                    gol.Partida = partida;
                    var contagem = 0;
                    var goleador = rnd.Next(1, totalgols);
                    foreach (var jog in clube2.Escalacao.OrderByDescending(x => x.Jogador.NotaUlt).ThenByDescending(x => x.Posicao).Take(5))
                    {
                        contagem = contagem + jog.HGol;
                        if (goleador <= contagem)
                        {
                            gol.Jogador = jog.Jogador;
                            break;
                        }
                    }
                    lstGols.Add(gol); /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR
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

            clube1.Dinheiro = clube1.Dinheiro + (publico * clube1.Ingresso);
            //clubeRepository.SaveOrUpdate(clube1);
            //clubeRepository.SaveOrUpdate(clube2);

            partida.Gol1 = gol1;
            partida.Gol2 = gol2;
            if (partida.Tipo == "TACA" && partida.Mao == 1)
                partida.Vencedor = null;
            else
                partida.Vencedor = vencedor;
            partida.Realizada = true;
            partida.Publico = publico;
            //partidaRepository.SaveOrUpdate(partida);

            ViewBag.Escalacao1 = clube1.Escalacao.ToList();
            ViewBag.Escalacao2 = clube2.Escalacao.ToList();
            partida.Gols = lstGols;

            return View(partida);
        }

        [Transaction]
        public ActionResult FuncaoAleatoria()
        {
            clubeQueryRepository.TirarTreinador(25);

            return RedirectToAction("Index", "Engine");
        }
    }
}
