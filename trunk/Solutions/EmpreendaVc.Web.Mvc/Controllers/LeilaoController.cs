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

    public class LeilaoController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly IClubeRepository clubeQueryRepository;
        private readonly INHibernateRepository<Jogador> jogadorRepository;
        private readonly INHibernateRepository<Controle> controleRepository;
        private readonly INHibernateRepository<Jogador> leilaoRepository;
        private readonly INHibernateRepository<JogadorLeilaoOferta> jogadorleilaoofertaRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly IPartidaRepository partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<JogadorPedidoLeilao> jogadorpedidoleilaoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;
        private readonly INHibernateRepository<Escalacao> escalacaoRepository;
        private readonly INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository;
        private readonly INHibernateRepository<JogadorOferta> jogadorofertaRepository;
        private readonly INHibernateRepository<Staff> staffRepository;
        private readonly INHibernateRepository<JogadorLeilao> jogadorleilaoRepository;

        public LeilaoController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            IClubeRepository clubeQueryRepository,
            INHibernateRepository<Jogador> jogadorRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<Jogador> leilaoRepository,
            INHibernateRepository<JogadorLeilaoOferta> jogadorleilaoofertaRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedidoLeilao> jogadorpedidoleilaoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository,
            INHibernateRepository<Escalacao> escalacaoRepository,
            INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository,
            INHibernateRepository<JogadorOferta> jogadorofertaRepository,
            INHibernateRepository<Staff> staffRepository,
            INHibernateRepository<JogadorLeilao> jogadorleilaoRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.clubeQueryRepository = clubeQueryRepository;
            this.jogadorRepository = jogadorRepository;
            this.controleRepository = controleRepository;
            this.leilaoRepository = leilaoRepository;
            this.jogadorleilaoofertaRepository = jogadorleilaoofertaRepository;
            this.divisaoRepository = divisaoRepository;
            this.partidaRepository = partidaRepository;
            this.golRepository = golRepository;
            this.divisaotabelaRepository = divisaotabelaRepository;
            this.jogadorpedidoleilaoRepository = jogadorpedidoleilaoRepository;
            this.usuarioofertaRepository = usuarioofertaRepository;
            this.noticiaRepository = noticiaRepository;
            this.escalacaoRepository = escalacaoRepository;
            this.jogadorhistoricoRepository = jogadorhistoricoRepository;
            this.jogadorofertaRepository = jogadorofertaRepository;
            this.staffRepository = staffRepository;
            this.jogadorleilaoRepository = jogadorleilaoRepository;
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
        public ActionResult Busca(int? p)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            ViewBag.Clube = usuario.Clube;
            ViewBag.Page = p.HasValue ? p.Value : 1;

            if (usuario.Staffs.Where(x => x.Tipo == 1).Count() == 0)
            {
                ViewBag.Olheiro = false;
                return View();
            }

            ViewBag.Olheiro = true;

            var jogadorfiltro = new JogadorFiltroView();

            if (Session["JogadorLeilaoFiltroView"] != null)
            {
                jogadorfiltro = (JogadorFiltroView)Session["JogadorLeilaoFiltroView"];

                var lstJogador = jogadorleilaoRepository.GetAll()
                            .Where(x => x.Clube != null && x.Clube.Id != usuario.Clube.Id && x.Estagio < 2);

                if (!string.IsNullOrEmpty(jogadorfiltro.Nome))
                    lstJogador = lstJogador.Where(x => x.Jogador.Nome.Contains(jogadorfiltro.Nome.ToUpper()));
                if (jogadorfiltro.Posicao != 0)
                    lstJogador = lstJogador.Where(x => x.Jogador.Posicao == jogadorfiltro.Posicao);
                if (jogadorfiltro.Ordenacao == 0)
                    lstJogador = lstJogador.OrderBy(x => x.Jogador.Nome);
                else if (jogadorfiltro.Ordenacao == 1)
                    lstJogador = lstJogador.OrderBy(x => x.Jogador.Posicao);
                else if (jogadorfiltro.Ordenacao == 2)
                    lstJogador = lstJogador.OrderByDescending(x => x.Jogador.Posicao);

                var skip = 0;

                if (p.HasValue)
                {
                    if (p.Value > 1)
                        skip = 40 * (p.Value - 1);
                }

                lstJogador = lstJogador.Skip(skip).Take(40);

                ViewBag.Resultado = lstJogador.ToList();
            }
            else
            {
                ViewBag.Resultado = new List<JogadorLeilao>();
            }

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

            Session["JogadorLeilaoFiltroView"] = jogadorfiltro;

            var lstJogador = jogadorleilaoRepository.GetAll()
                            .Where(x => x.Clube != null && x.Clube.Id != usuario.Clube.Id);

            if (!string.IsNullOrEmpty(jogadorfiltro.Nome))
                lstJogador = lstJogador.Where(x => x.Jogador.Nome.Contains(jogadorfiltro.Nome.ToUpper()));
            if (jogadorfiltro.Posicao != 0)
                lstJogador = lstJogador.Where(x => x.Jogador.Posicao == jogadorfiltro.Posicao);
            if (jogadorfiltro.Ordenacao == 0)
                lstJogador = lstJogador.OrderBy(x => x.Jogador.Nome);
            else if (jogadorfiltro.Ordenacao == 1)
                lstJogador = lstJogador.OrderBy(x => x.Jogador.Posicao);
            else if (jogadorfiltro.Ordenacao == 2)
                lstJogador = lstJogador.OrderByDescending(x => x.Jogador.Posicao);
            
            lstJogador = lstJogador.Take(40);

            ViewBag.Resultado = lstJogador.ToList();
            ViewBag.Clube = usuario.Clube;
            ViewBag.Page = 1;
            ViewBag.Olheiro = true;
            //ViewBag.Controle = controleRepository.GetAll().FirstOrDefault();

            return View(jogadorfiltro);
        }

        [Authorize]
        public ActionResult JogadorOferta(int id)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogadorleilao = jogadorleilaoRepository.Get(id);
            var jogadoroferta = jogadorleilaoofertaRepository.GetAll().Where(x => x.JogadorLeilao.Id == jogadorleilao.Id).FirstOrDefault();

            if (jogadoroferta == null)
                jogadoroferta = new JogadorLeilaoOferta();

            jogadoroferta.JogadorLeilao = jogadorleilao;

            ViewBag.Clube = usuario.Clube;

            return View(jogadoroferta);
        }

        [Authorize]
        [HttpPost]
        [Transaction]
        public ActionResult JogadorOferta(int id, FormCollection form)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogadorleilao = jogadorleilaoRepository.Get(id);
            var jogadoroferta = jogadorleilaoofertaRepository.GetAll().Where(x => x.JogadorLeilao.Id == jogadorleilao.Id).FirstOrDefault();

            if (jogadoroferta == null)
                jogadoroferta = new JogadorLeilaoOferta();

            TryUpdateModel(jogadoroferta, form);

            jogadoroferta.JogadorLeilao = jogadorleilao;
            jogadoroferta.Clube = usuario.Clube;

            //valida
            if (jogadoroferta.Salario < 1)
            {
                TempData["MsgAlerta"] = "Você precisa selecionar um valor para salário.";
                return View(jogadoroferta);
            }
            if (jogadoroferta.Contrato < 1)
            {
                TempData["MsgAlerta"] = "Você precisa selecionar a duração do contrato.";
                return View(jogadoroferta);
            }            

            if (jogadoroferta.IsValid())
            {
                if (jogadorleilao.Clube != null && jogadorleilao.Clube.Usuario != null)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = Util.Util.LinkaClube(jogadoroferta.Clube) + " apresentou uma proposta no leilão de " + Util.Util.LinkaJogador(jogadorleilao.Jogador) + ".";
                    noticia.Usuario = jogadorleilao.Clube.Usuario;

                    noticiaRepository.SaveOrUpdate(noticia);
                }

                var pontos = Convert.ToInt32((jogadoroferta.Salario - jogadorleilao.Jogador.Salario) / 10000);

                if (jogadorleilao.Jogador.Clube != null)
                {
                    pontos = pontos + (jogadorleilao.Jogador.Clube.Divisao.Numero - jogadoroferta.Clube.Divisao.Numero);
                    pontos = pontos + (jogadorleilao.Jogador.Situacao - 1) + jogadorleilao.Jogador.Satisfacao;
                }

                jogadoroferta.Pontos = pontos;
                jogadorleilaoofertaRepository.SaveOrUpdate(jogadoroferta);

                TempData["MsgOk"] = "Proposta feita com sucesso!";
                return RedirectToAction("Index", "Jogador", new { id = jogadorleilao.Jogador.Id });
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

            var lstjogadoroferta = jogadorleilaoofertaRepository.GetAll().Where(x => x.Clube.Id == usuario.Clube.Id && x.Estagio < 3);

            ViewBag.lstofertas = jogadorofertaRepository.GetAll().Where(x => x.Clube.Id == usuario.Clube.Id && x.Estagio < 3);
            return View(lstjogadoroferta);
        }

        [Authorize]
        public ActionResult MeusJogadores()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstjogadorleilao = jogadorleilaoRepository.GetAll().Where(x => x.Clube != null && x.Clube.Id == usuario.Clube.Id && x.Estagio < 3);

            return View(lstjogadorleilao);
        } 

        [Authorize]
        [Transaction]
        public ActionResult CancelarLeilao(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogadoroleilao = jogadorleilaoRepository.Get(id);

            jogadorleilaoRepository.Delete(jogadoroleilao);

            return RedirectToAction("MeusJogadores", "Leilao");
        }

        [Authorize]
        [Transaction]
        public ActionResult CancelarOferta(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogadoroferta = jogadorleilaoofertaRepository.Get(id);

            jogadorleilaoofertaRepository.Delete(jogadoroferta);

            return RedirectToAction("MinhasPropostas", "Leilao");
        }

        [Authorize]
        public ActionResult JogadorPedidoLeilao(int id)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogador = jogadorRepository.Get(id);
            var jogadorpedido = new JogadorPedidoLeilao();

            jogadorpedido.Jogador = jogador;

            ViewBag.Clube = usuario.Clube;

            return View(jogadorpedido);
        }

        [Authorize]
        [HttpPost]
        [Transaction]
        public ActionResult JogadorPedidoLeilao(int id, FormCollection form)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogador = jogadorRepository.Get(id);
            var jogadorpedido = new JogadorPedidoLeilao();

            TryUpdateModel(jogadorpedido, form);

            jogadorpedido.Jogador = jogador;

            if (jogadorpedido.IsValid())
            {
                if (jogador.Clube != null && jogador.Clube.Usuario != null)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = Util.Util.LinkaClube(usuario.Clube) + " pediu que você levasse " + Util.Util.LinkaJogador(jogador) + " a leilão. Ele estaria disposto a pagar o valor de $" + jogadorpedido.Valor.ToString("N2") + ".";
                    noticia.Usuario = jogador.Clube.Usuario;

                    noticiaRepository.SaveOrUpdate(noticia);
                }
                else
                {
                    jogadorpedidoleilaoRepository.SaveOrUpdate(jogadorpedido);
                }                

                TempData["MsgOk"] = "Pedido efetuado com sucesso!";
                return RedirectToAction("Index", "Jogador", new { id = jogador.Id });
            }
            else
            {
                TempData["MsgErro"] = "Erro na validação dos campos!";
                return View(jogadorpedido);
            }
        }

        [Authorize]
        public ActionResult BotarLeilao(int id)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogador = jogadorRepository.Get(id);
            var jogadorleilao = jogadorleilaoRepository.GetAll().FirstOrDefault(x => x.Clube.Id == usuario.Clube.Id && x.Jogador.Id == jogador.Id);

            if (jogadorleilao == null)
                jogadorleilao = new JogadorLeilao();

            jogadorleilao.Jogador = jogador;

            ViewBag.Clube = usuario.Clube;

            return View(jogadorleilao);
        }

        [Authorize]
        [HttpPost]
        [Transaction]
        public ActionResult BotarLeilao(int id, FormCollection form)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var jogador = jogadorRepository.Get(id);
            var jogadorleilao = new JogadorLeilao();

            TryUpdateModel(jogadorleilao, form);

            jogadorleilao.Jogador = jogador;
            jogadorleilao.Clube = usuario.Clube;
            jogadorleilao.Dia = controle.Dia;
            jogadorleilao.Estagio = 2;

            if (jogadorleilao.IsValid())
            {
                jogadorleilaoRepository.SaveOrUpdate(jogadorleilao);

                TempData["MsgOk"] = "Leilão de jogador efetuado com sucesso!";
                return RedirectToAction("Index", "Jogador", new { id = jogador.Id });
            }
            else
            {
                TempData["MsgErro"] = "Erro na validação dos campos!";
                return View(jogadorleilao);
            }
        }
    }
}
