using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DACO.Web.Mvc.Controllers
{
    public class ErroController : Controller
    {
        //
        // GET: /Erro/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NaoEncontrado()
        {
            var lastEx = (Exception)System.Web.HttpContext.Current.Application["TheException"];
            string msgModel = "";
            if (lastEx != null)
            {
                msgModel = lastEx.ToString();
                ViewData.Add("MsgModel", msgModel);
            }
            return View();
        }

        public ActionResult Problema()
        {
            var lastEx = (Exception)System.Web.HttpContext.Current.Application["TheException"];
            string msgModel = "";
            if (lastEx != null) {
                msgModel = lastEx.ToString();
                ViewData.Add("MsgModel", msgModel);
            }
            return View();
        }

    }
}
