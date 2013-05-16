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
        private readonly IUsuarioRepository usuarioRepository;
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
        private readonly INHibernateRepository<Historico> historicoRepository;

        public AdmController(IUsuarioRepository usuarioRepository,
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
            INHibernateRepository<Escalacao> escalacaoRepository,
            INHibernateRepository<Historico> historicoRepository)
        {
            this.usuarioRepository = usuarioRepository;
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
            this.historicoRepository = historicoRepository;
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

        #region Usuario
        
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

        #endregion

        #region Clube

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

        public ActionResult EditClube(int? id)
        {
            if ((bool)Session["ADM"])
            {
                DataBind();
                var clube = new Clube();

                if (id.HasValue)
                    clube = clubeRepository.Get(id.Value);

                return View(clube);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [Transaction]
        public ActionResult EditClube(FormCollection form, int id)
        {
            if ((bool)Session["ADM"])
            {                
                var clube = new Clube();

                if (id != 0)
                    clube = clubeRepository.Get(id);

                TryUpdateModel(clube, form);

                if (clube.IsValid())
                {
                    clube.Nome = clube.Nome.ToUpper();
                    clubeRepository.SaveOrUpdate(clube);
                    return RedirectToAction("GridClube");
                }

                DataBind();
                return View(clube);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #endregion

        public void DataBind()
        {
            ViewBag.Divisao = divisaoRepository.GetAll().OrderBy(x => x.Numero).Select(x => new SelectListItem() { Text = x.Nome, Value = x.Numero.ToString() });
        }
    }
}