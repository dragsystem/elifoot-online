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

    public class MensagemController : ControllerCustom
    {
        public ActionResult Index()
        {
            ViewBag.MsgErro = TempData["MsgErro"];
            ViewBag.MsgOk = TempData["MsgOk"];
            ViewBag.MsgAlerta = TempData["MsgAlerta"];

            TempData["MsgErro"] = null;
            TempData["MsgOk"] = null;
            TempData["MsgAlerta"] = null;

            return View();
        }
    }
}
