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

            if (usuario.Clube == null)
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

            ViewBag.Resposta = false;
            ViewBag.Controle = controle;

            return View(patrocinio);
        }

        [HttpPost]
        [Transaction]
        [Authorize]
        public ActionResult PatrocinioOferta(int id, FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var controle = controleRepository.GetAll().FirstOrDefault();

            if (usuario.Clube == null)
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

            if (form["Propor"] == "1")
            {
                var divisaoalvo = (patrocinio.DivisaoAlvo - clube.Divisao.Numero) < 1 ? 1 : patrocinio.DivisaoAlvo - clube.Divisao.Numero; 
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

                        return View(patrocinioclube);
                    }
                }
                else
                {
                    TempData["MsgErro"] = patrocinio.Nome + " aceitou sua proposta, CONFIRME para finalizar.";
                    ViewBag.Resposta = true;

                    return View(patrocinioclube);
                }
            }
            else
            {
                patrocinioclube.Clube = clube;
                patrocinioclube.Patrocinio = patrocinio;
                patrocinioclubeRepository.SaveOrUpdate(patrocinioclube);

                TempData["MsgOk"] = "PARABÉNS! Você fechou contrato de $" + patrocinioclube.Valor.ToString("N2") + " por " + patrocinioclube.Contrato + " ano(s) com " + patrocinio.Nome + ".";
                return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
            }
        }   
       
        [Authorize]
        public ActionResult PatrocinioRenovar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var patrocinio = patrocinioRepository.Get(id);

            if (patrocinio.Usuario == null || patrocinio.Usuario.Id != usuario.Id)
                return RedirectToAction("Index", "Conta");

            return View(patrocinio);
        }

        [Authorize]
        [Transaction]
        [HttpPost]
        public ActionResult PatrocinioRenovar(FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var patrocinio = patrocinioRepository.Get(Convert.ToInt32(form["Jogador"]));

            if (patrocinio.Usuario != null || patrocinio.Usuario.Id != usuario.Id)
                return RedirectToAction("Index", "Conta");

            var salario = Convert.ToDecimal(form["Salario"]);
            var contrato = Convert.ToInt32(form["Contrato"]);

            if (patrocinio != null)
            {
                var salariomin = (patrocinio.Salario / 100) * 120;

                if (salario >= salariomin)
                {
                    patrocinio.Salario = salario;
                    patrocinio.Contrato = contrato;
                    patrocinioRepository.SaveOrUpdate(patrocinio);

                    TempData["MsgOk"] = patrocinio.Nome + " aceitou sua proposta e renovou por " + contrato + " temporada(s)!";
                    return RedirectToAction("Index", "Patrocinio", new { id = patrocinio.Id });
                }
                else
                {
                    TempData["MsgErro"] = patrocinio.Nome + " rejeitou sua proposta de renovação!";
                }
            }
            return View(patrocinio);
        }

        [Authorize]
        public ActionResult PatrocinioDispensar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var patrocinio = patrocinioRepository.Get(id);

            if (patrocinio.Usuario == null || patrocinio.Usuario.Id != usuario.Id)
                return RedirectToAction("Index", "Conta");

            if (usuario.Clube != null)
                ViewBag.Clube = true;
            else
                ViewBag.Clube = false;

            return View(patrocinio);
        }        
        
        [Authorize]
        [Transaction]
        public ActionResult PatrocinioDispensarConfirma(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var patrocinio = patrocinioRepository.Get(id);

            if (patrocinio.Usuario == null || patrocinio.Usuario.Id != usuario.Id)
                return RedirectToAction("Index", "Conta");

            patrocinio.Usuario = null;
            patrocinio.Salario = 0;
            patrocinio.Contrato = 0;
            patrocinioRepository.SaveOrUpdate(patrocinio);

            if (usuario.Clube != null)
            {
                var clube = usuario.Clube;
                clube.Dinheiro = clube.Dinheiro - ((patrocinio.Salario * 2) * patrocinio.Contrato);
                clubeRepository.SaveOrUpdate(clube);
            }

            TempData["MsgOk"] = patrocinio.Nome + " foi dispensado com sucesso!";
            return RedirectToAction("Index", "Patrocinio", new { id = id });
        }          
    }
}
