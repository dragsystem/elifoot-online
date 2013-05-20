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
    using System.Web.Security;
    using EmpreendaVc.Web.Mvc.Controllers.ViewModels;

    public class ClubeController : ControllerCustom
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
        private readonly INHibernateRepository<Partida> partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<JogadorPedido> jogadorpedidoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;

        public ClubeController(IUsuarioRepository usuarioRepository,
            IClubeRepository clubeQueryRepository,
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
        }

        public ActionResult Index(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube.Id == id)
                return RedirectToAction("Plantel", "Clube");

            var clube = clubeRepository.Get(id);
            usuario.Clube.Partidas = clubeQueryRepository.PartidasClube(clube.Id);

            return View(clube);
        }

        [Authorize]
        public ActionResult Plantel()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstUsuarioOferta = usuarioofertaRepository.GetAll().Where(x => x.Usuario.Id == usuario.Id);
            if (lstUsuarioOferta.Count() > 0)
                return RedirectToAction("UsuarioOferta", "Conta");

            var lstJogadorPedido = jogadorpedidoRepository.GetAll().Where(x => x.Jogador.Clube.Id == usuario.Clube.Id);
            if (lstUsuarioOferta.Count() > 0)
                return RedirectToAction("JogadorPedido", "Clube");

            usuario.Clube.Partidas = clubeQueryRepository.PartidasClube(usuario.Clube.Id);
            
            return View(usuario.Clube);
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
            var jogador = jogadorRepository.Get(Convert.ToInt32(form["Valor"]));
            var clube = jogador.Clube;

            var valor = Convert.ToDecimal(form["Valor"]);

            if (jogador != null)
            {
                if (valor >= (jogador.Salario + (jogador.Salario / 100) * 30))
                {
                    jogador.Salario = valor;
                    jogador.Contrato = true;
                    jogadorRepository.SaveOrUpdate(jogador);

                    TempData["MsgOk"] = jogador.Nome + " aceitou sua proposta e renovou até o final da temporada!";
                    return RedirectToAction("Plantel", "Clube");
                }
            }

            TempData["MsgErro"] = jogador.Nome + " rejeitou o contrato proposto.";
            return View(jogador);
        }

        [Authorize]
        public ActionResult JogadorVender(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var jogador = jogadorRepository.Get(id);
            var clube = jogador.Clube;
            var verificaleilao = leilaoRepository.GetAll().Where(x => x.Jogador.Id == jogador.Id && x.OfertaVencedora == null);

            if (usuario.Clube == null || usuario.Clube.Id != clube.Id)
                return RedirectToAction("Index", "Conta");

            if (clube.Jogadores.Count() < 15)
            {
                TempData["MsgErro"] = "Você já possui o número mínimo de jogadores.";
                return View();
            }
            else if (verificaleilao.Count() > 0)
            {
                TempData["MsgErro"] = "Este jogador já está sendo leiloado. Se quiser alterar cancele o leilão atual.";
                return View();
            }
            else
            {
                var leilao = new Leilao() { Jogador = jogador };
                return View(leilao);                
            }
        }

        [Authorize]
        [Transaction]
        [HttpPost]
        public ActionResult JogadorVender(FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var leilao = new Leilao();
            var controle = controleRepository.GetAll().FirstOrDefault();

            TryUpdateModel(leilao, form);

            if (leilao.IsValid())
            {
                leilao.Dia = controle.Dia++;
                leilao.Espontaneo = false;
                leilaoRepository.SaveOrUpdate(leilao);

                TempData["MsgOk"] = leilao.Jogador.Nome + " será vendido no próximo leilão!";
                return RedirectToAction("Plantel", "Clube");
            }

            TempData["MsgErro"] = "Erro na validação dos campos!";
            return View(leilao);
        }

        public ActionResult ClassificacaoClube(int iddivisao)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            ViewBag.Clube = usuario.Clube;

            var divisao = divisaoRepository.Get(iddivisao);

            return View(divisao);
        }

        public ActionResult Classificacao(int iddivisao)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            ViewBag.Clube = usuario.Clube != null ? usuario.Clube : new Clube();

            var divisao = divisaoRepository.Get(iddivisao);

            ViewBag.lstDivisao = divisaoRepository.GetAll();

            return View(divisao);
        }

        public ActionResult Taca(int? rodada)
        {
            var partidas = partidaRepository.GetAll().Where(x => x.Tipo == "TACA");

            var controle = controleRepository.GetAll().FirstOrDefault();
            var rodadareal = controle.Taca / 2;

            if (rodada.HasValue)
                rodadareal = rodada.Value;

            ViewBag.Rodada = rodadareal;
            ViewBag.Mao = partidas.Where(x => x.Rodada == rodadareal && !x.Realizada && x.Mao == 1).Count() > 0 ? 1 : 2;

            ViewBag.lstDivisao = divisaoRepository.GetAll();

            return View(partidas);
        }

        [Authorize]
        public ActionResult JogadorPedido()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var pedidos = jogadorpedidoRepository.GetAll().Where(x => x.Jogador.Clube.Id == usuario.Clube.Id);

            if (pedidos.Count() > 0)
                return View(pedidos);
            else
                return View();
        }

        [Authorize]
        [Transaction]
        public ActionResult JogadorPedidoResposta(int id, bool resposta)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var pedido = jogadorpedidoRepository.Get(id);
            var jogador = jogadorRepository.Get(pedido.Jogador.Id);

            if (resposta)
            {
                jogador.Contrato = true;
                jogador.Salario = pedido.Salario;
                jogadorRepository.SaveOrUpdate(jogador);                
            }
            else
            {
                var rnd = new Random();

                if (rnd.Next(0, 2) == 1)
                {
                    var controle = controleRepository.GetAll().FirstOrDefault();
                    var leilao = new Leilao();
                    leilao.Dia = controle.Dia++;
                    leilao.Espontaneo = true;
                    leilao.Jogador = jogador;
                    leilao.Valor = jogador.H * 50000;

                    leilaoRepository.SaveOrUpdate(leilao);
                }
            }

            jogadorpedidoRepository.Delete(pedido);
            return RedirectToAction("Plantel", "Clube");
        }

        [Authorize]
        public ActionResult Calendario()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstPartidas = clubeQueryRepository.PartidasClube(usuario.Clube.Id).OrderBy(x => x.Dia);

            ViewBag.Clube = usuario.Clube;

            return View(lstPartidas);
        }

        [Authorize]
        public ActionResult Transferencias()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstLeilao = leilaoRepository.GetAll().Where(x => x.OfertaVencedora != null).OrderBy(x => x.Dia);

            ViewBag.Clube = usuario.Clube;

            return View(lstLeilao);
        }

        [Authorize]
        public ActionResult Resultados(int? divisao, int? rodada)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var divisaoresult = usuario.Clube.Divisao.Numero;

            if (divisao.HasValue)
                divisaoresult = divisao.Value;

            var lstPartidas = new List<Partida>();

            if (divisaoresult == 0)
            {
                var numero = Convert.ToInt32(divisaoresult);
                ViewBag.Competicao = divisaoresult + "ª DIVISÃO";

                if (rodada.HasValue)
                {
                    ViewBag.Rodada = rodada.Value + "ª Rodada";
                    lstPartidas = partidaRepository.GetAll().Where(x => x.Divisao.Numero == numero && x.Rodada == rodada.Value && x.Realizada).ToList();
                }
                else
                {
                    lstPartidas = partidaRepository.GetAll().Where(x => x.Divisao.Numero == numero && x.Realizada).OrderByDescending(x => x.Rodada).ToList();
                    var ultrodada = lstPartidas.First().Rodada;
                    ViewBag.Rodada = ultrodada + "ª Rodada";
                    lstPartidas = lstPartidas.Where(x => x.Rodada == ultrodada).ToList();
                }
            }
            else
            {
                ViewBag.Competicao = "TAÇA";

                if (rodada.HasValue)
                {
                    var ultrodada = rodada.Value;
                    if (ultrodada == 16)
                        ViewBag.Rodada = "1ª Eliminatória";
                    else if (ultrodada == 8)
                        ViewBag.Rodada = "Oitavas de Final";
                    else if (ultrodada == 4)
                        ViewBag.Rodada = "Quartas de Final";
                    else if (ultrodada == 2)
                        ViewBag.Rodada = "Semi Final";
                    else if (ultrodada == 1)
                        ViewBag.Rodada = "FINAL";

                    lstPartidas = partidaRepository.GetAll().Where(x => x.Tipo == "TACA" && x.Rodada == ultrodada && x.Realizada).ToList();
                }
                else
                {
                    lstPartidas = partidaRepository.GetAll().Where(x => x.Tipo == "TACA" && x.Realizada).OrderBy(x => x.Rodada).ToList();
                    var ultrodada = lstPartidas.First().Rodada;
                    if (ultrodada == 16)
                        ViewBag.Rodada = "1ª Eliminatória";
                    else if (ultrodada == 8)
                        ViewBag.Rodada = "Oitavas de Final";
                    else if (ultrodada == 4)
                        ViewBag.Rodada = "Quartas de Final";
                    else if (ultrodada == 2)
                        ViewBag.Rodada = "Semi Final";
                    else if (ultrodada == 1)
                        ViewBag.Rodada = "FINAL";
                    lstPartidas = lstPartidas.Where(x => x.Rodada == ultrodada).ToList();
                }
            }

            ViewBag.Clube = usuario.Clube;

            return View(lstPartidas);
        }        
    }
}
