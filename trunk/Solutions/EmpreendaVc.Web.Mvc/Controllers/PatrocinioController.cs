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

    public class PatrocinioController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly IClubeRepository clubeQueryRepository;
        private readonly INHibernateRepository<Patrocinio> patrocinioRepository;
        private readonly INHibernateRepository<PatrocinioClube> patrocinioclubeRepository;
        private readonly INHibernateRepository<PatrocinioRecusa> patrociniorecusaRepository;
        private readonly INHibernateRepository<Controle> controleRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly IPartidaRepository partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;

        public PatrocinioController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            IClubeRepository clubeQueryRepository,
            INHibernateRepository<Patrocinio> patrocinioRepository,
            INHibernateRepository<PatrocinioClube> patrocinioclubeRepository,
            INHibernateRepository<PatrocinioRecusa> patrociniorecusaRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.clubeQueryRepository = clubeQueryRepository;
            this.patrocinioRepository = patrocinioRepository;
            this.patrocinioclubeRepository = patrocinioclubeRepository;
            this.patrociniorecusaRepository = patrociniorecusaRepository;
            this.controleRepository = controleRepository;
            this.divisaoRepository = divisaoRepository;
            this.partidaRepository = partidaRepository;
            this.golRepository = golRepository;
            this.divisaotabelaRepository = divisaotabelaRepository;
            this.usuarioofertaRepository = usuarioofertaRepository;
            this.noticiaRepository = noticiaRepository;
        }

        [Authorize]
        public ActionResult Index(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            ViewBag.Clube = usuario.Clube;
            ViewBag.Controle = controleRepository.GetAll().FirstOrDefault();

            var patrocinio = patrocinioRepository.Get(id);

            return View(patrocinio);
        }

        [Authorize]
        public ActionResult Busca()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null)
                return RedirectToAction("Index", "Conta");

            var lst = patrocinioRepository.GetAll().OrderBy(x => x.Nome);

            ViewBag.Clube = usuario.Clube;

            return View(lst);
        }

        [Authorize]
        public ActionResult PatrocinioOferta(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var controle = controleRepository.GetAll().FirstOrDefault();

            if (usuario.Clube == null || usuario.Clube.PatrocinioClubes.Where(x => x.Patrocinio.Id == id).Count() > 0)
                return RedirectToAction("Index", "Conta");

            var clube = usuario.Clube;
            var patrocinio = patrocinioRepository.Get(id);

            if (clube.PatrocinioRecusas.Where(x => x.Patrocinio.Id == patrocinio.Id).Count() >= 3)
            {
                TempData["MsgErro"] = patrocinio.Nome + " não tem interesse em negociar com seu clube.";
                return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
            }

            if (patrocinio.Tipo == 1)
            {
                if (clube.PatrocinioClubes.Where(x => x.Tipo == 1 || x.Tipo == 2).Count() > 1)
                {
                    TempData["MsgErro"] = "Você já possui patrocinador MASTER e MANGA. Para negociar com " + patrocinio.Nome + ", primeiro encerre pelo menos um dos contratos atuais.";
                    return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
                }
            }
            else
            {
                if (clube.PatrocinioClubes.Where(x => x.Tipo == 3).Count() > 0)
                {
                    TempData["MsgErro"] = "Você já possui um FORNECEDOR DE MATERIAL. Para negociar com " + patrocinio.Nome + ", primeiro encerre o contratos atual.";
                    return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
                }
            }

            var patrocinioclube = new PatrocinioClube();
            patrocinioclube.Patrocinio = patrocinio;
            patrocinioclube.Clube = clube;

            Session["PatrocinioClube"] = null;
            ViewBag.Resposta = false;
            ViewBag.Controle = controle;

            return View(patrocinioclube);
        }

        [HttpPost]
        [Transaction]
        [Authorize]
        public ActionResult PatrocinioOferta(int id, FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var controle = controleRepository.GetAll().FirstOrDefault();

            if (usuario.Clube == null || usuario.Clube.PatrocinioClubes.Where(x => x.Patrocinio.Id == id).Count() > 0)
                return RedirectToAction("Index", "Conta");

            var clube = usuario.Clube;
            var patrocinio = patrocinioRepository.Get(id);

            if (patrocinio.Tipo == 1)
            {
                if (clube.PatrocinioClubes.Where(x => x.Tipo == 1 || x.Tipo == 2).Count() > 1)
                {
                    TempData["MsgErro"] = "Você já possui patrocinador MASTER e MANGA. Para negociar com " + patrocinio.Nome + ", primeiro encerre pelo menos um dos contratos atuais.";
                    return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
                }
            }
            else
            {
                if (clube.PatrocinioClubes.Where(x => x.Tipo == 3).Count() > 0)
                {
                    TempData["MsgErro"] = "Você já possui um FORNECEDOR DE MATERIAL. Para negociar com " + patrocinio.Nome + ", primeiro encerre o contratos atual.";
                    return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
                }
            }

            var patrocinioclube = new PatrocinioClube();

            TryUpdateModel(patrocinioclube, form);

            ViewBag.Resposta = false;
            ViewBag.Controle = controle;
            
            if (Session["PatrocinioClube"] == null)
            {
                //valida
                if (patrocinioclube.Tipo < 1)
                {
                    TempData["MsgAlerta"] = "Você precisa selecionar um tipo de contrato.";
                    Session["PatrocinioClube"] = null;
                    patrocinioclube.Clube = clube;
                    patrocinioclube.Patrocinio = patrocinio;
                    return View(patrocinioclube);
                }
                if (patrocinioclube.Valor < 1)
                {
                    TempData["MsgAlerta"] = "Você precisa selecionar um valor.";
                    Session["PatrocinioClube"] = null;
                    patrocinioclube.Clube = clube;
                    patrocinioclube.Patrocinio = patrocinio;
                    return View(patrocinioclube);
                }
                if (patrocinioclube.Contrato < 1)
                {
                    TempData["MsgAlerta"] = "Você precisa selecionar a duração do contrato.";
                    Session["PatrocinioClube"] = null;
                    patrocinioclube.Clube = clube;
                    patrocinioclube.Patrocinio = patrocinio;
                    return View(patrocinioclube);
                }

                var divisaoalvo = (clube.Divisao.Numero - (patrocinio.DivisaoAlvo - 1)) < 1 ? 1 : (clube.Divisao.Numero - (patrocinio.DivisaoAlvo - 1)); 
                decimal valormax = (patrocinio.ValorMax / divisaoalvo);

                if (patrocinioclube.Valor > valormax)
                {
                    if (clube.PatrocinioRecusas.Where(x => x.Patrocinio.Id == patrocinio.Id).Count() >= 2)
                    {
                        TempData["MsgErro"] = patrocinio.Nome + " rejeitou sua última proposta e encerrou as negociações.";
                        var patrociniorecusa = new PatrocinioRecusa();
                        patrociniorecusa.Clube = clube;
                        patrociniorecusa.Patrocinio = patrocinio;
                        patrociniorecusa.Ano = controle.Ano;
                        patrociniorecusaRepository.SaveOrUpdate(patrociniorecusa);

                        return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
                    }
                    else
                    {
                        TempData["MsgErro"] = patrocinio.Nome + " rejeitou sua última proposta, mas ainda está disposto a negociar.";

                        var patrociniorecusa = new PatrocinioRecusa();
                        patrociniorecusa.Clube = clube;
                        patrociniorecusa.Patrocinio = patrocinio;
                        patrociniorecusa.Ano = controle.Ano;
                        patrociniorecusaRepository.SaveOrUpdate(patrociniorecusa);

                        Session["PatrocinioClube"] = null;
                        patrocinioclube.Clube = clube;
                        patrocinioclube.Patrocinio = patrocinio;
                        return View(patrocinioclube);
                    }
                }
                else
                {
                    TempData["MsgOk"] = patrocinio.Nome + " aceitou sua proposta, CONFIRME para finalizar.";
                    ViewBag.Resposta = true;

                    patrocinioclube.Clube = clube;
                    patrocinioclube.Patrocinio = patrocinio;

                    Session["PatrocinioClube"] = patrocinioclube;
                    return View(patrocinioclube);
                }
            }
            else
            {
                patrocinioclube = (PatrocinioClube)Session["PatrocinioClube"];
                patrocinioclube.Clube = clube;
                patrocinioclube.Patrocinio = patrocinio;
                patrocinioclubeRepository.SaveOrUpdate(patrocinioclube);

                Session["PatrocinioClube"] = null;
                TempData["MsgOk"] = "PARABÉNS! Você fechou contrato de $" + patrocinioclube.Valor.ToString("N2") + " por " + patrocinioclube.Contrato + " ano(s) com " + patrocinio.Nome + ".";
                return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
            }
        }

        [Authorize]
        [Transaction]
        public ActionResult PatrocinioOfertaCancelar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var controle = controleRepository.GetAll().FirstOrDefault();

            if (usuario.Clube == null || usuario.Clube.PatrocinioClubes.Where(x => x.Patrocinio.Id == id).Count() > 0)
                return RedirectToAction("Index", "Conta");

            var clube = usuario.Clube;
            var patrocinio = patrocinioRepository.Get(id);

            for (int i = 0; i < 3; i++)
            {
                var patrociniorecusa = new PatrocinioRecusa();
                patrociniorecusa.Clube = clube;
                patrociniorecusa.Patrocinio = patrocinio;
                patrociniorecusa.Ano = controle.Ano;
                patrociniorecusaRepository.SaveOrUpdate(patrociniorecusa);
            }

            Session["PatrocinioClube"] = null;

            return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
        }
        
        [Authorize]
        public ActionResult PatrocinioEncerrar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null || usuario.Clube.PatrocinioClubes.Where(x => x.Patrocinio.Id == id).Count() == 0)
                return RedirectToAction("Index", "Conta");

            var clube = usuario.Clube;
            var patrocinioclube = patrocinioclubeRepository.Get(clube.PatrocinioClubes.Where(x => x.Patrocinio.Id == id).FirstOrDefault().Id);

            return View(patrocinioclube);
        }        
        
        [Authorize]
        [Transaction]
        public ActionResult PatrocinioEncerrarConfirma(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario.Clube == null || usuario.Clube.PatrocinioClubes.Where(x => x.Patrocinio.Id == id).Count() == 0)
                return RedirectToAction("Index", "Conta");

            var clube = usuario.Clube;
            var patrocinioclube = patrocinioclubeRepository.Get(id);

            patrocinioclubeRepository.Delete(patrocinioclube);

            clube.Dinheiro = clube.Dinheiro - ((patrocinioclube.Valor / 2) * patrocinioclube.Contrato);
            clubeRepository.SaveOrUpdate(clube);

            TempData["MsgOk"] = "Contrato com " + patrocinioclube.Patrocinio.Nome + " encerrado com sucesso!";
            return RedirectToAction("Index", "Patrocinio", new { id = patrocinioclube.Patrocinio.Id });
        }          
    }
}
