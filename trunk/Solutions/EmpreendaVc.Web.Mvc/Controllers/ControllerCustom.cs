using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.Net;
using EmpreendaVc.Web.Mvc.Controllers.ViewModels;

namespace EmpreendaVc.Web.Mvc.Controllers
{
    public class ControllerCustom : Controller
    {
        public ILog ObjLog
        {
            get { return log4net.LogManager.GetLogger("LogInFile"); }
        }

        public JsonResult JsonNoContent()
        {
            Response.StatusCode = (int)HttpStatusCode.NoContent;
            return Json(null);
        }

        public JsonResult JsonError(ErrorDictionary errors)
        {
            //Response.StatusCode = 418;
            //Response.StatusCode = 410;
            return Json(new { errors }, JsonRequestBehavior.AllowGet);
        }

    }
}
