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
        private readonly INHibernateRepository<Escalacao> escalacaoRepository;

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
            INHibernateRepository<Noticia> noticiaRepository,
            INHibernateRepository<Escalacao> escalacaoRepository)
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
        }

        public ActionResult Index(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube.Id == id)
                return RedirectToAction("Plantel", "Clube");

            var clube = clubeRepository.Get(id);
            clube.Partidas = clubeQueryRepository.PartidasClube(clube.Id);

            return View(clube);
        }

        [Authorize]
        public ActionResult Plantel()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
            {
                TempData["MsgErro"] = "Você não é treinador de nenhum clube. Aguarde uma proposta.";
                return RedirectToAction("Index", "Conta");
            }

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
            var jogador = jogadorRepository.Get(Convert.ToInt32(form["Jogador"]));
            var clube = jogador.Clube;

            var valor = Convert.ToDecimal(form["Salario"]);

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
            var controle = controleRepository.GetAll().FirstOrDefault();
            var jogador = jogadorRepository.Get(Convert.ToInt32(form["Jogador"]));

            var leilao = new Leilao();

            TryUpdateModel(leilao, form);

            if (leilao.IsValid())
            {
                leilao.Dia = controle.Dia++;
                leilao.Jogador = jogador;
                leilao.Espontaneo = false;
                leilao.Clube = leilao.Jogador.Clube;
                leilaoRepository.SaveOrUpdate(leilao);

                TempData["MsgOk"] = jogador.Nome + " será vendido no próximo leilão!";
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
            var usuario = authenticationService.GetUserAuthenticated();

            var partidas = partidaRepository.GetAll().Where(x => x.Tipo == "TACA");

            ViewBag.Clube = usuario.Clube != null ? usuario.Clube : new Clube();

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
            var controle = controleRepository.GetAll().FirstOrDefault();
            ViewBag.Dia = controle.Dia;

            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lstPartidas = clubeQueryRepository.PartidasClube(usuario.Clube.Id).OrderBy(x => x.Dia);

            ViewBag.Clube = usuario.Clube;            

            return View(lstPartidas);
        }

        [Authorize]
        [Transaction]
        public ActionResult Formacao(string formacao)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var clube = usuario.Clube;

            var escalacao = clube.Escalacao;

            if (clube.Formacao != formacao)
            {
                var lstescalacao = escalacaoRepository.GetAll().Where(x => x.Clube.Id == clube.Id);

                clube.Formacao = formacao;
                clubeRepository.SaveOrUpdate(clube);

                foreach (var item in lstescalacao)
                {
                    item.Clube = null;
                    item.Jogador = null;
                    escalacaoRepository.Delete(item);
                }
                           
                escalacao = EscalarTime(clube);
            }

            ViewBag.Formacao = formacao;

            return View(escalacao);
        }

        [Authorize]
        public ActionResult Transferencias()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var controle = controleRepository.GetAll().FirstOrDefault();
            ViewBag.Dia = controle.Dia;

            var lstLeilao = leilaoRepository.GetAll().Where(x => x.OfertaVencedora != null).OrderBy(x => x.Dia);

            ViewBag.Clube = usuario.Clube;

            return View(lstLeilao);
        }

        [Authorize]
        public ActionResult Resultados(int? divisao, int? rodada, int? taca)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var divisaoresult = usuario.Clube.Divisao.Numero;

            if (divisao.HasValue)
                divisaoresult = divisao.Value;

            var lstPartidas = new List<Partida>();            

            if (!taca.HasValue)
            {
                var numero = divisaoresult;
                ViewBag.Divisao = divisaoresult;
                ViewBag.Competicao = divisaoresult + "ª DIVISÃO";
                
                if (rodada.HasValue)
                {
                    ViewBag.Rodada = rodada.Value + "ª Rodada";
                    lstPartidas = partidaRepository.GetAll().Where(x => x.Divisao.Numero == numero && x.Rodada == rodada.Value && x.Realizada).ToList();
                }
                else
                {
                    lstPartidas = partidaRepository.GetAll().Where(x => x.Tipo == "DIVISAO" && x.Divisao.Numero == numero && x.Realizada).OrderByDescending(x => x.Rodada).ToList();
                    var ultrodada = lstPartidas.First().Rodada;
                    ViewBag.Rodada = ultrodada + "ª Rodada";
                    lstPartidas = lstPartidas.Where(x => x.Rodada == ultrodada).ToList();
                }
            }
            else
            {
                ViewBag.Divisao = 0;
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
                    var ultrodada = lstPartidas.Count() > 0 ? lstPartidas.First().Rodada : 16;
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

        [Transaction]
        public List<Escalacao> EscalarTime(Clube clube)
        {
            var lstescalacao = new List<Escalacao>();

            //GOLEIRO
            var escalacao = new Escalacao();
            escalacao.Clube = clube;
            escalacao.Posicao = 1;
            escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 1).OrderByDescending(x => x.H).FirstOrDefault();
            escalacaoRepository.SaveOrUpdate(escalacao);
            lstescalacao.Add(escalacao);

            //LATERAL-DIREITO
            if (clube.Formacao.Substring(0, 1) != "3")
            {
                escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 2;
                escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 2).OrderByDescending(x => x.H).FirstOrDefault();
                escalacaoRepository.SaveOrUpdate(escalacao);
                lstescalacao.Add(escalacao);
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
                lstescalacao.Add(escalacao);
            }

            //LATERAL-ESQUERDO
            if (clube.Formacao.Substring(0, 1) != "3")
            {
                escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 4;
                escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 4).OrderByDescending(x => x.H).FirstOrDefault();
                escalacaoRepository.SaveOrUpdate(escalacao);
                lstescalacao.Add(escalacao);
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
                lstescalacao.Add(escalacao);
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
                lstescalacao.Add(escalacao);
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
                lstescalacao.Add(escalacao);
            }

            return lstescalacao;
        }
    }
}
