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

    public class HomeController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IAuthenticationService authenticationService;

        public HomeController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
        }

        public ActionResult Index()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario != null)
                ViewBag.Usuario = usuario;

            return View();
        }

        public ActionResult Contato()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            var contato = new Contato();

            if (usuario != null)
            {
                contato.Usuario = usuario;
                contato.Nome = usuario.NomeCompleto;
                contato.Email = usuario.Email;
            }

            return View(contato);
        }

        [HttpPost]
        public ActionResult Contato(FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            var contato = new Contato();

            TryUpdateModel(contato, form);
            
            if (usuario != null)
            {
                contato.Usuario = usuario;
                contato.Nome = usuario.NomeCompleto;
                contato.Email = usuario.Email;
            }

            try
            {
                if (contato.IsValid())
                {
                    new SendMailController().Contato(contato).Deliver();
                    TempData["MsgOk"] = "Contato enviado com sucesso! Responderemos no prazo de 24h.";
                    return RedirectToAction("Contato");
                }
            }
            catch (Exception ex)
            {
                ObjLog.Error("HomeController(Contato): " + ex.ToString());
            }

            TempData["MsgErro"] = "Erro na validação dos campos.";

            return RedirectToAction("Contato");
        }
    }
}
