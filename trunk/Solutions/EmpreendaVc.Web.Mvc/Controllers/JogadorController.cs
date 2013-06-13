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

    public class JogadorController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly IClubeRepository clubeQueryRepository;
        private readonly INHibernateRepository<Jogador> jogadorRepository;
        private readonly INHibernateRepository<Controle> controleRepository;
        private readonly INHibernateRepository<Jogador> leilaoRepository;
        private readonly INHibernateRepository<JogadorOferta> jogadorofertaRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly IPartidaRepository partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<JogadorPedido> jogadorpedidoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;

        public JogadorController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            IClubeRepository clubeQueryRepository,
            INHibernateRepository<Jogador> jogadorRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<Jogador> leilaoRepository,
            INHibernateRepository<JogadorOferta> jogadorofertaRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedido> jogadorpedidoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.clubeQueryRepository = clubeQueryRepository;
            this.jogadorRepository = jogadorRepository;
            this.controleRepository = controleRepository;
            this.leilaoRepository = leilaoRepository;
            this.jogadorofertaRepository = jogadorofertaRepository;
            this.divisaoRepository = divisaoRepository;
            this.partidaRepository = partidaRepository;
            this.golRepository = golRepository;
            this.divisaotabelaRepository = divisaotabelaRepository;
            this.jogadorpedidoRepository = jogadorpedidoRepository;
            this.usuarioofertaRepository = usuarioofertaRepository;
            this.noticiaRepository = noticiaRepository;
        }

        [Authorize]
        public ActionResult Index(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");


            ViewBag.Clube = usuario.Clube;
            ViewBag.Controle = controleRepository.GetAll().FirstOrDefault();

            var jogador = jogadorRepository.Get(id);

            return View(jogador);
        }

        [Authorize]
        public ActionResult Busca()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogadorfiltro = new JogadorFiltroView();

            if (Session["JogadorFiltroView"] != null)
            {
                jogadorfiltro = (JogadorFiltroView)Session["JogadorFiltroView"];

                var lstJogador = jogadorRepository.GetAll()
                            .Where(x => x.Clube.Id != usuario.Clube.Id && !x.Temporario);

                if (jogadorfiltro.Nome != "")
                    lstJogador = lstJogador.Where(x => x.Nome.Contains(jogadorfiltro.Nome));
                if (jogadorfiltro.Posicao != 0)
                    lstJogador = lstJogador.Where(x => x.Posicao == jogadorfiltro.Posicao);
                if (jogadorfiltro.Situacao != 0)
                    lstJogador = lstJogador.Where(x => x.Situacao == jogadorfiltro.Situacao);
                if (jogadorfiltro.Contrato != 0)
                {
                    if (jogadorfiltro.Contrato == (-1))
                        lstJogador = lstJogador.Where(x => x.Clube == null);
                    else
                        lstJogador = lstJogador.Where(x => x.Contrato == 1);
                }
                if (jogadorfiltro.Ordenacao == 0)
                    lstJogador = lstJogador.OrderBy(x => x.Nome);
                else if (jogadorfiltro.Ordenacao == 1)
                    lstJogador = lstJogador.OrderBy(x => x.Posicao);
                else if (jogadorfiltro.Ordenacao == 2)
                    lstJogador = lstJogador.OrderByDescending(x => x.Posicao);
                else if (jogadorfiltro.Ordenacao == 3)
                    lstJogador = lstJogador.OrderByDescending(x => x.NotaMedia);

                ViewBag.Resultado = lstJogador;
            }
            else
            {
                ViewBag.Resultado = new List<Jogador>();
            }

            ViewBag.Clube = usuario.Clube;

            return View(jogadorfiltro);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Busca(FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogadorfiltro = new JogadorFiltroView();

            TryUpdateModel(jogadorfiltro, form);

            Session["JogadorFiltroView"] = jogadorfiltro;

            var lstJogador = jogadorRepository.GetAll()
                            .Where(x => x.Clube.Id != usuario.Clube.Id);

            if (jogadorfiltro.Nome != "")
                lstJogador = lstJogador.Where(x => x.Nome.Contains(jogadorfiltro.Nome));
            if (jogadorfiltro.Posicao != 0)
                lstJogador = lstJogador.Where(x => x.Posicao == jogadorfiltro.Posicao);
            if (jogadorfiltro.Situacao != 0)
                lstJogador = lstJogador.Where(x => x.Situacao == jogadorfiltro.Situacao);
            if (jogadorfiltro.Contrato != 0)
            {
                if (jogadorfiltro.Contrato == (-1))
                    lstJogador = lstJogador.Where(x => x.Clube == null);
                else
                    lstJogador = lstJogador.Where(x => x.Contrato == 1);
            }
            if (jogadorfiltro.Ordenacao == 0)
                lstJogador = lstJogador.OrderBy(x => x.Nome);
            else if (jogadorfiltro.Ordenacao == 1)
                lstJogador = lstJogador.OrderBy(x => x.Posicao);
            else if (jogadorfiltro.Ordenacao == 2)
                lstJogador = lstJogador.OrderByDescending(x => x.Posicao);
            else if (jogadorfiltro.Ordenacao == 3)
                lstJogador = lstJogador.OrderByDescending(x => x.NotaMedia);

            ViewBag.Resultado = lstJogador;
            ViewBag.Clube = usuario.Clube;
            //ViewBag.Controle = controleRepository.GetAll().FirstOrDefault();

            return View(jogadorfiltro);
        }

        [Authorize]
        public ActionResult JogadorOferta(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogador = jogadorRepository.Get(id);
            var jogadoroferta = jogadorofertaRepository.GetAll().Where(x => x.Jogador.Id == jogador.Id && x.Clube.Id == usuario.Clube.Id).FirstOrDefault();

            if (jogadoroferta == null)
                jogadoroferta = new JogadorOferta();

            if (jogador.Clube != null)
                jogadoroferta.Tipo = 1;
            else
                jogadoroferta.Tipo = 2;

            jogadoroferta.Jogador = jogador;

            return View(jogadoroferta);
        }

        [Authorize]
        [HttpPost]
        [Transaction]
        public ActionResult JogadorOferta(int idjogador, FormCollection form)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogador = jogadorRepository.Get(idjogador);
            var jogadoroferta = jogadorofertaRepository.GetAll().Where(x => x.Jogador.Id == jogador.Id && x.Clube.Id == usuario.Clube.Id).FirstOrDefault();

            if (jogadoroferta == null)
                jogadoroferta = new JogadorOferta();

            TryUpdateModel(jogadoroferta, form);

            jogadoroferta.Jogador = jogador;

            if (jogador.Clube != null)
            {
                jogadoroferta.Tipo = 1;
                jogadoroferta.Estagio = 1;
            }
            else
            {
                jogadoroferta.Tipo = 2;
                jogadoroferta.Estagio = 2;
            }

            jogadoroferta.Clube = usuario.Clube;

            if (jogadoroferta.IsValid())
            {
                if (jogador.Clube.Usuario != null)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = jogadoroferta.Clube.Nome + " apresentou uma proposta por " + jogador.Nome + ". <br /><br />Entre em Propostas para responder.";
                    noticia.Usuario = jogador.Clube.Usuario;

                    noticiaRepository.SaveOrUpdate(noticia);
                }

                var pontos = Convert.ToInt32((jogadoroferta.Salario - jogador.Salario) / 10000);

                if (jogador.Clube != null)
                {
                    pontos = pontos + (jogador.Clube.Divisao.Numero - jogadoroferta.Clube.Divisao.Numero);
                    pontos = pontos + (jogador.Situacao - 1);
                }

                jogadoroferta.Pontos = pontos;
                jogadorofertaRepository.SaveOrUpdate(jogadoroferta);

                TempData["MsgOk"] = "Proposta feita com sucesso!";
                return RedirectToAction("Index", "Jogador", new { id = jogador.Id });
            }
            else
            {
                TempData["MsgErro"] = "Erro na validação dos campos!";
                return View(jogadoroferta);
            }
        }
        
        [Authorize]
        public ActionResult MinhasPropostas()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstjogadoroferta = jogadorofertaRepository.GetAll().Where(x => x.Clube.Id == usuario.Clube.Id);

            return View(lstjogadoroferta);
        }

        [Authorize]
        public ActionResult MeusJogadores()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstjogadoroferta = jogadorofertaRepository.GetAll().Where(x => x.Jogador.Clube.Id == usuario.Clube.Id);

            return View(lstjogadoroferta);
        }

        [Authorize]
        [Transaction]
        public ActionResult CancelarVenda(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogadoroferta = jogadorofertaRepository.Get(id);

            jogadorofertaRepository.Delete(jogadoroferta);

            return RedirectToAction("MeusJogadores", "Jogador");
        }

        [Authorize]
        [Transaction]
        public ActionResult CancelarOferta(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogadoroferta = jogadorofertaRepository.Get(id);

            jogadorofertaRepository.Delete(jogadoroferta);

            return RedirectToAction("MinhasPropostas", "Jogador");
        }

        [Authorize]
        public ActionResult JogadorRenovar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var jogador = jogadorRepository.Get(id);
            var clube = jogador.Clube;

            if (usuario.Clube == null || usuario.Clube.Id != clube.Id)
                return RedirectToAction("Index", "Conta");

            return View(jogador);
        }

        [Authorize]
        [Transaction]
        [HttpPost]
        public ActionResult JogadorRenovar(FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var jogador = jogadorRepository.Get(Convert.ToInt32(form["Jogador"]));
            var clube = jogador.Clube;

            var salario = Convert.ToDecimal(form["Salario"]);
            var contrato = Convert.ToInt32(form["Contrato"]);

            if (jogador != null)
            {
                int pontos = Convert.ToInt32((salario - jogador.Salario) / 10000);
                pontos = pontos - jogador.Situacao;

                if (pontos > 0)
                {
                    jogador.Salario = salario;
                    jogador.Contrato = contrato;
                    jogadorRepository.SaveOrUpdate(jogador);

                    TempData["MsgOk"] = jogador.Nome + " aceitou sua proposta e renovou por " + contrato + " temporada(s)!";
                    return RedirectToAction("Plantel", "Clube");
                }
                else
                {
                    TempData["MsgErro"] = jogador.Nome + " rejeitou sua proposta de renovação!";
                }
            }
            return View(jogador);
        }

        [Authorize]
        public ActionResult JogadorDispensar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var jogador = jogadorRepository.Get(id);
            var clube = jogador.Clube;

            if (usuario.Clube == null || usuario.Clube.Id != clube.Id)
                return RedirectToAction("Index", "Conta");

            return View(jogador);
        }

        [HttpGet]
        [Transaction]
        public JsonResult AlteraSituacao(int id, int situacao)
        {
            var result = "";

            var jogador = jogadorRepository.Get(id);

            jogador.Situacao = situacao;
            jogadorRepository.SaveOrUpdate(jogador);

            result = "ok";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult _MenuJogador(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            ViewBag.Clube = usuario.Clube;
            ViewBag.Controle = controleRepository.GetAll().FirstOrDefault();

            var jogador = jogadorRepository.Get(id);

            return View(jogador);
        }
            
    }
}
