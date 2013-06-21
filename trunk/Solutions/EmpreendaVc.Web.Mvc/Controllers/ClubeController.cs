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

    public class ClubeController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IClubeRepository clubeQueryRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly INHibernateRepository<Jogador> jogadorRepository;
        private readonly INHibernateRepository<Controle> controleRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly IPartidaRepository partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<JogadorPedido> jogadorpedidoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;
        private readonly INHibernateRepository<Escalacao> escalacaoRepository;
        private readonly INHibernateRepository<Artilheiro> artilheiroRepository;

        public ClubeController(IUsuarioRepository usuarioRepository,
            IClubeRepository clubeQueryRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            INHibernateRepository<Jogador> jogadorRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedido> jogadorpedidoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository,
            INHibernateRepository<Escalacao> escalacaoRepository,
            INHibernateRepository<Artilheiro> artilheiroRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.clubeQueryRepository = clubeQueryRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.jogadorRepository = jogadorRepository;
            this.controleRepository = controleRepository;
            this.divisaoRepository = divisaoRepository;
            this.partidaRepository = partidaRepository;
            this.golRepository = golRepository;
            this.divisaotabelaRepository = divisaotabelaRepository;
            this.jogadorpedidoRepository = jogadorpedidoRepository;
            this.usuarioofertaRepository = usuarioofertaRepository;
            this.noticiaRepository = noticiaRepository;
            this.escalacaoRepository = escalacaoRepository;
            this.artilheiroRepository = artilheiroRepository;
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

            
            //if (lstUsuarioOferta.Count() > 0)
            //    return RedirectToAction("UsuarioOferta", "Conta");

            if (usuario.Clube == null)
            {
                TempData["MsgErro"] = "Você não é treinador de nenhum clube. Aguarde uma proposta.";
                return RedirectToAction("Index", "Conta");
            }

            var lstJogadorPedido = jogadorpedidoRepository.GetAll().Where(x => x.Jogador.Clube.Id == usuario.Clube.Id);
            //if (lstUsuarioOferta.Count() > 0)
            //    return RedirectToAction("JogadorPedido", "Clube");

            usuario.Clube.Partidas = clubeQueryRepository.PartidasClube(usuario.Clube.Id);
            
            return View(usuario.Clube);
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

            ViewBag.lstArtilheiros = artilheiroRepository.GetAll().Where(x => x.Clube != null && x.Clube.Divisao.Id == divisao.Id).OrderByDescending(x => x.Divisao).Take(10).ToList();

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

            ViewBag.lstArtilheiros = artilheiroRepository.GetAll().OrderByDescending(x => x.Taca).Take(10).ToList();

            ViewBag.lstDivisao = divisaoRepository.GetAll();

            return View(partidas);
        }

        [Authorize]
        public ActionResult Emprego()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            var lst = clubeRepository.GetAll().Where(x => x.Usuario == null).OrderBy(x => x.Divisao.Numero).ThenBy(x => x.Nome);

            return View(lst);
        }

        [Authorize]
        public ActionResult Proposta(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if ((usuario.Clube != null && usuario.Clube.Id == id) || usuario.IdUltimoClube == id)
                return RedirectToAction("Index", "Conta");

            var clube = clubeRepository.Get(id);

            var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Numero;
            var repgeral = clube.Divisao.Numero < ultdivisao ? (80 / clube.Divisao.Numero) : 0;

            if (usuario.ReputacaoGeral >= repgeral && usuario.IdUltimoClube != clube.Id && usuario.DelayTroca == 0)
                ViewBag.Aceita = true;
            else
                ViewBag.Aceita = false;

            return View(clube);
        }

        [Authorize]
        [Transaction]
        public ActionResult PropostaAceitar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var clube = clubeRepository.Get(id);

            if ((usuario.Clube != null && usuario.Clube.Id == id) || usuario.IdUltimoClube == id || clube.Usuario != null)
                return RedirectToAction("Index", "Conta");

            clube.Usuario = usuario;
            clube.ReputacaoAI = 30;
            clubeRepository.SaveOrUpdate(clube);

            usuario.Clube = clube;
            usuario.DelayTroca = 5;
            usuario.Reputacao = 30;
            usuarioRepository.SaveOrUpdate(usuario);

            return View(clube);
        }

        //[Authorize]
        //public ActionResult JogadorPedido()
        //{
        //    var usuario = authenticationService.GetUserAuthenticated();

        //    if (usuario.Clube == null)
        //        return RedirectToAction("Index", "Conta");

        //    var pedidos = jogadorpedidoRepository.GetAll().Where(x => x.Jogador.Clube.Id == usuario.Clube.Id);

        //    if (pedidos.Count() > 0)
        //        return View(pedidos);
        //    else
        //        return View();
        //}

        //[Authorize]
        //[Transaction]
        //public ActionResult JogadorPedidoResposta(int id, bool resposta)
        //{
        //    var usuario = authenticationService.GetUserAuthenticated();
        //    var pedido = jogadorpedidoRepository.Get(id);
        //    var jogador = jogadorRepository.Get(pedido.Jogador.Id);

        //    if (resposta)
        //    {
        //        jogador.Contrato = true;
        //        jogador.Salario = pedido.Salario;
        //        jogadorRepository.SaveOrUpdate(jogador);                
        //    }
        //    else
        //    {
        //        var rnd = new Random();

        //        if (rnd.Next(0, 2) == 1)
        //        {
        //            var controle = controleRepository.GetAll().FirstOrDefault();
        //            var leilao = new Leilao();
        //            leilao.Dia = controle.Dia++;
        //            leilao.Espontaneo = true;
        //            leilao.Jogador = jogador;
        //            leilao.Valor = jogador.H * 50000;

        //            leilaoRepository.SaveOrUpdate(leilao);
        //        }
        //    }

        //    jogadorpedidoRepository.Delete(pedido);
        //    return RedirectToAction("Plantel", "Clube");
        //}

        [Authorize]
        public ActionResult Calendario(int? id)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            ViewBag.Dia = controle.Dia;

            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            ViewBag.Clube = usuario.Clube;

            if (id.HasValue)
            {
                var lstPartidas = clubeQueryRepository.PartidasClube(id.Value).OrderBy(x => x.Dia);
                ViewBag.ClubeId = id;
                return View(lstPartidas);
            }
            else
            {
                var lstPartidas = clubeQueryRepository.PartidasClube(usuario.Clube.Id).OrderBy(x => x.Dia);
                ViewBag.ClubeId = 0;
                return View(lstPartidas);
            }            
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

                if (lstescalacao.Count() != 11)
                {
                    foreach (var item in lstescalacao)
                    {
                        item.Clube = null;
                        item.Jogador = null;
                        escalacaoRepository.Delete(item);
                    }

                    var lstnova = CriarFormacao(clube);
                    foreach (var item in lstnova)
                    {
                        escalacaoRepository.SaveOrUpdate(item);
                    }
                }
                else
                {
                    var lstnova = CriarFormacao(clube);
                    var i = 0;

                    foreach (var item in lstescalacao)
                    {
                        item.Posicao = lstnova[i].Posicao;
                        if (item.Jogador != null)
                        {
                            item.H = Util.Util.RetornaHabilidadePosicao(item.Jogador, item.Posicao);
                        }
                        escalacaoRepository.SaveOrUpdate(item);

                        i++;
                    }
                }

                clubeRepository.SaveOrUpdate(clube);
            }

            return RedirectToAction("Plantel", "Clube");
        }

        public List<Escalacao> CriarFormacao(Clube clube)
        {
            var lstescalacao = new List<Escalacao>();
            var qnt = 0;

            //GOLEIRO
            var escalacao = new Escalacao();
            escalacao.Clube = clube;
            escalacao.Posicao = 1;
            lstescalacao.Add(escalacao);

            //LATERAL-DIREITO
            if (clube.Formacao.Substring(0, 1) != "3")
            {
                escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 2;
                lstescalacao.Add(escalacao);
            }

            //ZAGUEIROS
            qnt = 3;
            if (clube.Formacao.Substring(0, 1) == "4")
                qnt = 2;

            for (int i = 0; i < qnt; i++)
            {
                escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 3;
                lstescalacao.Add(escalacao);
            }

            //LATERAL-ESQUERDO
            if (clube.Formacao.Substring(0, 1) != "3")
            {
                escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 4;
                lstescalacao.Add(escalacao);
            }

            //VOLANTE
            qnt = Convert.ToInt32(clube.Formacao.Substring(1, 1));
            for (int i = 0; i < qnt; i++)
            {
                escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 5;
                lstescalacao.Add(escalacao);
            }

            //MEIA OFENSIVO
            qnt = Convert.ToInt32(clube.Formacao.Substring(2, 1));
            for (int i = 0; i < qnt; i++)
            {
                escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 6;
                lstescalacao.Add(escalacao);
            }

            //ATACANTES
            qnt = Convert.ToInt32(clube.Formacao.Substring(3, 1));
            for (int i = 0; i < qnt; i++)
            {
                escalacao = new Escalacao();
                escalacao.Clube = clube;
                escalacao.Posicao = 7;
                lstescalacao.Add(escalacao);
            }

            return lstescalacao;
        }

        [Transaction]
        [HttpGet]//[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult EscalaJogador(int id, int idescalacao)
        {
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var usuario = authenticationService.GetUserAuthenticated();
            var clube = usuario.Clube;
            var escalacao = clube.Escalacao;

            if (idescalacao > 0)
            {
                var escalacaoitem = clube.Escalacao.FirstOrDefault(x => x.Id == idescalacao);
                escalacaoitem.Jogador = jogadorRepository.Get(id);
                escalacaoitem.H = Util.Util.RetornaHabilidadePosicao(escalacaoitem.Jogador, escalacaoitem.Posicao);
                escalacaoRepository.SaveOrUpdate(escalacaoitem);
            }
            else
            {
                var escalacaoitem = clube.Escalacao.FirstOrDefault(x => x.Jogador != null && x.Jogador.Id == id);
                escalacaoitem.Jogador = null;
                escalacaoitem.H = 0;
                escalacaoRepository.SaveOrUpdate(escalacaoitem);
            }            

            var novaescalacao = escalacaoRepository.GetAll().Where(x => x.Clube.Id == clube.Id).OrderBy(x => x.Posicao).Select(x => new EscalacaoView() { 
                                                                                                                Id = x.Id, 
                                                                                                                JogadorId = x.Jogador != null ? x.Jogador.Id : 0, 
                                                                                                                Posicao = Util.Util.RetornaPosicao(x.Posicao) 
                                                                                                                });

            return Json(oSerializer.Serialize(novaescalacao), JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        //public ActionResult Transferencias()
        //{
        //    var usuario = authenticationService.GetUserAuthenticated();

        //    if (usuario.Clube == null)
        //        return RedirectToAction("Index", "Conta");

        //    var controle = controleRepository.GetAll().FirstOrDefault();
        //    ViewBag.Dia = controle.Dia;

        //    var lstLeilao = leilaoRepository.GetAll().Where(x => x.OfertaVencedora != null).OrderBy(x => x.Dia);

        //    ViewBag.Clube = usuario.Clube;

        //    return View(lstLeilao);
        //}

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
                    ViewBag.RodadaNum = rodada.Value;
                    lstPartidas = partidaRepository.GetAll().Where(x => x.Tipo == "DIVISAO" && x.Divisao.Numero == numero && x.Rodada == rodada.Value && x.Realizada).ToList();
                }
                else
                {
                    lstPartidas = partidaRepository.GetAll().Where(x => x.Tipo == "DIVISAO" && x.Divisao.Numero == numero && x.Realizada).OrderByDescending(x => x.Rodada).ToList();
                    var ultrodada = lstPartidas.Count() > 0 ? lstPartidas.First().Rodada : 1;
                    ViewBag.Rodada = ultrodada + "ª Rodada";
                    ViewBag.RodadaNum = ultrodada;
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

                    ViewBag.RodadaNum = ultrodada;
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

                    ViewBag.RodadaNum = ultrodada;
                }
            }

            ViewBag.Clube = usuario.Clube;

            ViewBag.lstDivisao = divisaoRepository.GetAll();

            return View(lstPartidas);
        }

        [Authorize]
        public ActionResult _Menu(int? id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            ViewBag.Usuario = usuario;

            var clube = usuario.Clube;

            if (id.HasValue)
                clube = clubeRepository.Get(id.Value);

            return View(clube);
        }

        [HttpGet]
        [Transaction]
        public JsonResult AlteraIngresso(decimal valor)
        {
            var result = "";
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return Json(result, JsonRequestBehavior.AllowGet);

            var clube = usuario.Clube;
            clube.Ingresso = valor;

            clubeRepository.SaveOrUpdate(clube);
            result = "ok";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[Transaction] FORMAÇÃO OLD
        //public List<Escalacao> EscalarTime(Clube clube)
        //{
        //    var lstescalacao = new List<Escalacao>();

        //    //GOLEIRO
        //    var escalacao = new Escalacao();
        //    escalacao.Clube = clube;
        //    escalacao.Posicao = 1;
        //    escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 1).OrderByDescending(x => x.H).FirstOrDefault();
        //    escalacaoRepository.SaveOrUpdate(escalacao);
        //    lstescalacao.Add(escalacao);

        //    //LATERAL-DIREITO
        //    if (clube.Formacao.Substring(0, 1) != "3")
        //    {
        //        escalacao = new Escalacao();
        //        escalacao.Clube = clube;
        //        escalacao.Posicao = 2;
        //        escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 2).OrderByDescending(x => x.H).FirstOrDefault();
        //        escalacaoRepository.SaveOrUpdate(escalacao);
        //        lstescalacao.Add(escalacao);
        //    }

        //    //ZAGUEIROS
        //    var zagueiros = clube.Jogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).Take(3);
        //    if (clube.Formacao.Substring(0, 1) == "4")
        //        zagueiros = clube.Jogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).Take(2);

        //    foreach (var zag in zagueiros)
        //    {
        //        escalacao = new Escalacao();
        //        escalacao.Clube = clube;
        //        escalacao.Posicao = 3;
        //        escalacao.Jogador = zag;
        //        escalacaoRepository.SaveOrUpdate(escalacao);
        //        lstescalacao.Add(escalacao);
        //    }

        //    //LATERAL-ESQUERDO
        //    if (clube.Formacao.Substring(0, 1) != "3")
        //    {
        //        escalacao = new Escalacao();
        //        escalacao.Clube = clube;
        //        escalacao.Posicao = 4;
        //        escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 4).OrderByDescending(x => x.H).FirstOrDefault();
        //        escalacaoRepository.SaveOrUpdate(escalacao);
        //        lstescalacao.Add(escalacao);
        //    }

        //    //VOLANTE
        //    var volantes = clube.Jogadores.Where(x => x.Posicao == 5).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(1, 1)));
        //    foreach (var vol in volantes)
        //    {
        //        escalacao = new Escalacao();
        //        escalacao.Clube = clube;
        //        escalacao.Posicao = 5;
        //        escalacao.Jogador = vol;
        //        escalacaoRepository.SaveOrUpdate(escalacao);
        //        lstescalacao.Add(escalacao);
        //    }

        //    //MEIA OFENSIVO
        //    var meias = clube.Jogadores.Where(x => x.Posicao == 6).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(2, 1)));
        //    foreach (var mei in meias)
        //    {
        //        escalacao = new Escalacao();
        //        escalacao.Clube = clube;
        //        escalacao.Posicao = 6;
        //        escalacao.Jogador = mei;
        //        escalacaoRepository.SaveOrUpdate(escalacao);
        //        lstescalacao.Add(escalacao);
        //    }

        //    //ATACANTES
        //    var atacantes = clube.Jogadores.Where(x => x.Posicao == 7).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(3, 1)));
        //    foreach (var ata in atacantes)
        //    {
        //        escalacao = new Escalacao();
        //        escalacao.Clube = clube;
        //        escalacao.Posicao = 7;
        //        escalacao.Jogador = ata;
        //        escalacaoRepository.SaveOrUpdate(escalacao);
        //        lstescalacao.Add(escalacao);
        //    }

        //    return lstescalacao;
        //}
    }
}
