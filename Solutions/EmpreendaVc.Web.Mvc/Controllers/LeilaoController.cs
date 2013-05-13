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

    public class LeilaoController : ControllerCustom
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

        public LeilaoController(IUsuarioRepository usuarioRepository,
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

        [Authorize]
        public ActionResult Index(int? nome, int? clube, int? divisao, int? posicao, int? h, int? valor)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstLeilao = leilaoRepository.GetAll().Where(x => x.Jogador.Clube.Id != usuario.Clube.Id).OrderBy(x => x.Jogador.Clube.Nome).ThenBy(x => x.Jogador.Clube.Divisao.Nome);

            ViewBag.Order = 0;

            if (nome.HasValue)
            {
                if (nome.Value == 1)
                {
                    ViewBag.Order = 1;
                    lstLeilao.OrderBy(x => x.Jogador.Nome);
                }
                else
                    lstLeilao.OrderByDescending(x => x.Jogador.Nome);
            }
            else if (clube.HasValue)
            {
                if (clube.Value == 1)
                {
                    ViewBag.Order = 1;
                    lstLeilao.OrderBy(x => x.Jogador.Clube.Nome);
                }
                else
                    lstLeilao.OrderByDescending(x => x.Jogador.Clube.Nome);
            }
            else if (divisao.HasValue)
            {
                if (divisao.Value == 1)
                {
                    ViewBag.Order = 1;
                    lstLeilao.OrderBy(x => x.Jogador.Clube.Divisao.Nome);
                }
                else
                    lstLeilao.OrderByDescending(x => x.Jogador.Clube.Divisao.Nome);
            }
            else if (posicao.HasValue)
            {
                if (posicao.Value == 1)
                {
                    ViewBag.Order = 1;
                    lstLeilao.OrderBy(x => x.Jogador.Posicao);
                }
                else
                    lstLeilao.OrderByDescending(x => x.Jogador.Posicao);
            }
            else if (h.HasValue)
            {
                if (h.Value == 1)
                {
                    ViewBag.Order = 1;
                    lstLeilao.OrderByDescending(x => x.Jogador.H);
                }
                else
                    lstLeilao.OrderBy(x => x.Jogador.H);
            }
            else if (valor.HasValue)
            {
                if (valor.Value == 1)
                {
                    ViewBag.Order = 1;
                    lstLeilao.OrderByDescending(x => x.Valor);
                }
                else
                    lstLeilao.OrderBy(x => x.Valor);
            }

            return View(lstLeilao);
        }

        [Authorize]
        public ActionResult Detalhe(int id)
        {
            var leilao = leilaoRepository.Get(id);

            return View(leilao);
        }

        [Authorize]
        public ActionResult LeilaoOferta(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");
            
            var leilao = leilaoRepository.Get(id);
            var leilaooferta = leilaoofertaRepository.GetAll().Where(x => x.Leilao.Id == leilao.Id && x.Clube.Id == usuario.Clube.Id).FirstOrDefault();

            if (leilaooferta == null)
                leilaooferta = new LeilaoOferta();

            return View(leilaooferta);
        }

        [Authorize]
        [HttpPost]
        public ActionResult LeilaoOferta(FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");
            
            var leilaooferta = new LeilaoOferta();

            TryUpdateModel(leilaooferta, form);

            leilaooferta.Clube = usuario.Clube;

            if (leilaooferta.IsValid())
            {
                leilaoofertaRepository.SaveOrUpdate(leilaooferta);

                TempData["MsgOk"] = "Proposta feita com sucesso!";
                return View(leilaooferta);
            }
            else
            {
                TempData["MsgErro"] = "Erro na validação dos campos!";
                return View(leilaooferta);
            }
        }
        
        [Authorize]
        public ActionResult MinhasOfertas()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstleilaooferta = leilaoofertaRepository.GetAll().Where(x => x.Clube.Id == usuario.Clube.Id);

            var lstleilao = new List<Leilao>();

            foreach (var item in lstleilaooferta)
            {
                lstleilao.Add(item.Leilao);
            }

            return View(lstleilao);
        }

        [Authorize]
        public ActionResult MeusJogadores()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstleilao = leilaoRepository.GetAll().Where(x => x.Jogador.Clube.Id == usuario.Clube.Id);
            
            return View(lstleilao);
        }

        [Authorize]
        public ActionResult CancelarOferta(int id, bool? detalhe)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstleilaooferta = leilaoofertaRepository.GetAll().Where(x => x.Leilao.Id == id);

            foreach (var item in lstleilaooferta)
            {
                leilaoofertaRepository.Delete(item);
            }

            return RedirectToAction("Detalhe", "Leilao", new { id = id });
        }
    }
}
