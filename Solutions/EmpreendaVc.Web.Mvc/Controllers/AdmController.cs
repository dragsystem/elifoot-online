using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmpreendaVc.Infrastructure.Queries.Usuarios;
using EmpreendaVc.Domain;
using SharpArch.NHibernate.Contracts.Repositories;
using EmpreendaVc.Infrastructure.Queries.Authentication;
using SharpArch.NHibernate.Web.Mvc;
using EmpreendaVc.Web.Mvc.Util;
using System.Drawing;
using System.IO;

namespace EmpreendaVc.Web.Mvc.Controllers
{
    public class AdmController : ControllerCustom
    {
        private readonly INHibernateRepository<Usuario> usuarioRepository;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Jogador> lutadorRepository;

        public AdmController(
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            INHibernateRepository<Usuario> usuarioRepository,
            INHibernateRepository<Jogador> lutadorRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.clubeRepository = clubeRepository;
            this.authenticationService = authenticationService;
            this.lutadorRepository = lutadorRepository;
        }

        [HttpGet]
        public ActionResult Login()
        {
            Session["ADM"] = false;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Login, string Senha)
        {
            Session["ADM"] = false;

            if (Login == "adm" && Senha == "2")
            {
                Session["ADM"] = true;
                return RedirectToAction("GridUsuario");
            }
            else
            {
                ModelState.AddModelError("", "Login ou Senha incorreta!");
                //var errors = ModelState.GetErrorDictionary();
                return View();
            }
        }

        public ActionResult GridUsuario()
        {
            if ((bool)Session["ADM"])
            {
                var lst = usuarioRepository.GetAll();
                return View(lst);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult GridClube()
        {
            if ((bool)Session["ADM"])
            {
                var lst = clubeRepository.GetAll();
                return View(lst);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult DetalheUsuario(int id)
        {
            if ((bool)Session["ADM"])
            {
                return View(usuarioRepository.Get(id));
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [Transaction]
        public ActionResult DetalheUsuario(int idUserAccount, FormCollection collection)
        {
            if ((bool)Session["ADM"])
            {
                var userModel = usuarioRepository.Get(idUserAccount);

                var senhanova = collection["Senha"].ToString();

                userModel.Senha = UsuarioCommand.HasPasswordToString(senhanova);

                try
                {
                    if (ModelState.IsValid && userModel.IsValid())
                    {
                        usuarioRepository.SaveOrUpdate(userModel);
                        TempData["SucessoAlteracao"] = true;
                    }
                }
                catch (Exception ex)
                {
                    ObjLog.Error(string.Format("AdmController(DetalheUsuario):{0}", ex.Message));
                    TempData["MessageError"] = true;
                }
                return View(userModel);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult DetalheClube(int id)
        {
            if ((bool)Session["ADM"])
            {
                return View(clubeRepository.Get(id));
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult GridLutador()
        {
            if ((bool)Session["ADM"])
            {
                var lst = lutadorRepository.GetAll();
                return View(lst);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [Transaction]
        public ActionResult GridLutador(FormCollection form)
        {
            if ((bool)Session["ADM"])
            {
                var lutador = new Jogador();

                TryUpdateModel(lutador, form);

                lutador.Nome = lutador.Nome.ToUpper();

                if (lutador.IsValid())
                    lutadorRepository.SaveOrUpdate(lutador);

                var lst = lutadorRepository.GetAll();
                return View(lst);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //[Transaction]
        //public ActionResult Apagar(int id, bool? IsUsuario, bool? IsEvento, bool? IsLuta, bool? IsLutador)
        //{
        //    if (IsEvento.HasValue)
        //    {
        //        var evento = eventoRepository.Get(id);

        //        if (evento != null)
        //        {
        //            var lutas = lutaRepository.GetAll().Where(x => x.Evento.Id == id);

        //            foreach (var luta in lutas)
        //            {
        //                var jogos = jogoRepository.GetAll().Where(x => x.Luta.Id == luta.Id);

        //                foreach (var jogo in jogos)
        //                {
        //                    jogoRepository.Delete(jogo);
        //                }

        //                lutaRepository.Delete(luta);
        //            }

        //            eventoRepository.Delete(evento);
        //        }

        //        TempData["SucessoAlteracao"] = true;
        //        return RedirectToAction("GridEvento");
        //    }
        //    if (IsLuta.HasValue)
        //    {
        //        var luta = lutaRepository.Get(id);
        //        var idevento = luta.Evento.Id;

        //        if (luta != null)
        //        {
        //            var jogos = jogoRepository.GetAll().Where(x => x.Luta.Id == luta.Id);

        //            foreach (var jogo in jogos)
        //            {
        //                jogoRepository.Delete(jogo);
        //            }

        //            lutaRepository.Delete(luta);
        //        }

        //        TempData["SucessoAlteracao"] = true;
        //        return RedirectToAction("DetalheEvento", new { id = idevento });
        //    }
        //    if (IsLutador.HasValue)
        //    {
        //        var lutador = lutadorRepository.Get(id);

        //        if (lutador != null)
        //        {
        //            var lutas = lutaRepository.GetAll().Where(x => x.Lutador1.Id == id || x.Lutador2.Id == id);

        //            foreach (var luta in lutas)
        //            {
        //                var jogos = jogoRepository.GetAll().Where(x => x.Luta.Id == luta.Id);

        //                foreach (var jogo in jogos)
        //                {
        //                    jogoRepository.Delete(jogo);
        //                }

        //                lutaRepository.Delete(luta);
        //            }

        //            lutadorRepository.Delete(lutador);
        //        }

        //        TempData["SucessoAlteracao"] = true;
        //        return RedirectToAction("GridLutador");
        //    }

        //    return RedirectToAction("GridUsuario");
        //}

        //public ActionResult FormLuta(int idevento, int? idluta)
        //{
        //    var evento = eventoRepository.Get(idevento);
        //    var luta = new Luta();

        //    if (idluta.HasValue)
        //        luta = lutaRepository.Get(idluta.Value);

        //    ViewBag.Evento = evento.Id;
        //    ViewBag.Lutadores = lutadorRepository.GetAll().OrderBy(x => x.Nome).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nome });

        //    return View("_FormLuta", luta);
        //}
        
        //[HttpPost]
        //[Transaction]
        //public ActionResult FormLuta(FormCollection form, int idevento, int idluta)
        //{
        //    var evento = eventoRepository.Get(idevento);
        //    var luta = new Luta();

        //    if (idluta != 0)
        //        luta = lutaRepository.Get(idluta);

        //    TryUpdateModel(luta, form);

        //    luta.Evento = evento;

        //    lutaRepository.SaveOrUpdate(luta);

        //    ViewBag.Lutadores = lutadorRepository.GetAll().OrderBy(x => x.Nome).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nome });

        //    return RedirectToAction("DetalheEvento", new { id = idevento });
        //}

        //[Transaction]
        //public ActionResult ApagarLuta(int id)
        //{
        //    var luta = lutaRepository.Get(id);

        //    var idevento = luta.Evento.Id;

        //    lutaRepository.Delete(luta);

        //    return RedirectToAction("DetalheEvento", new { id = idevento });
        //}
        //[Transaction]
        //public ActionResult EditIsActive(int idUsuario)
        //{
            //if ((bool)Session["ADM"])
            //{
            //    var Event = usuarioRepository.Get(idUsuario);

            //    try
            //    {
            //        if (Event.IsAtivo)
            //            Event.IsAtivo = false;
            //        else
            //            Event.IsAtivo = true;

            //        var erros = usuarioRepository.SaveOrUpdate(Event);

            //        if (erros.Count > 0)
            //        {
            //            foreach (var item in erros)
            //            {
            //                ModelState.AddModelError("", item);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        ObjLog.Error(string.Format("AdmController(EditIsVisivel):{0}", ex.Message));
            //        TempData["AdmError"] = true;
            //    }

            //    //usuarioRepository.TransactionRollback();
            //    return RedirectToAction("GridUsuario");
            //}
            //else
            //{
            //    return RedirectToAction("Login");
        //    }
        //}
    }
}