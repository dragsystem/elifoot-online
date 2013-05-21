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

    public class ContaController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Controle> controleRepository;

        public ContaController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Controle> controleRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.divisaoRepository = divisaoRepository;
            this.usuarioofertaRepository = usuarioofertaRepository;
            this.controleRepository = controleRepository;
        }

        public ActionResult Index()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            var lstUsuarioOferta = usuarioofertaRepository.GetAll().Where(x => x.Usuario.Id == usuario.Id);
            if (lstUsuarioOferta.Count() > 0)
                return RedirectToAction("UsuarioOferta", "Conta");

            return View(usuario);
        }

        public ActionResult Cadastro()
        {
            var usuario = new Usuario();

            return View(usuario);
        }        

        [HttpPost]
        [Transaction]
        public ActionResult Cadastro(FormCollection collection)
        {
            var usuario = new Usuario();
            TryUpdateModel(usuario, collection);

            if (usuario.NomeCompleto != null)
            {
                usuario.NomeCompleto = usuario.NomeCompleto.ToUpper();
                if (usuarioRepository.GetNome(usuario.NomeCompleto) != null)
                    TempData["MsgErro"] = "Nome já em uso";
            }
            else
                TempData["MsgErro"] = "Nome é um campo obrigatório";

            if (usuario.Email != null)
            {
                if (usuarioRepository.GetEmail(usuario.Email) != null)
                    TempData["MsgErro"] = "E-mail já em uso";

                if (usuario.Email.Contains("@@"))
                    TempData["MsgErro"] = "Favor preencher um E-mail válido";
            }
            else
                TempData["MsgErro"] = "E-mail é um campo obrigatório";

            if (usuario.Senha != null)
            {
                if (usuario.Senha.Length < 6)
                    TempData["MsgErro"] = "A senha precisa ter 6 ou mais caracteres";
            }
            else
            {
                TempData["MsgErro"] = "Favor preencher a senha";
            }

            var ConfirmaSenha = collection["ConfirmaSenha"].ToString();

            if (ConfirmaSenha == string.Empty)
                TempData["MsgErro"] = "Favor preencher a confirmação da senha";

            try
            {
                if (ModelState.IsValid && usuario.IsValid())
                {
                    if (usuario.Senha == ConfirmaSenha)
                    {

                        var result = this.usuarioRepository.SaveOrUpdate(usuario);

                        //define Guid do usuário
                        usuario.Guid = new Random().Next(1000000, 9999999).ToString();

                        this.usuarioRepository.SaveOrUpdate(usuario);

                        if (result.Count() == 0)
                        {
                            // _authenticationService.SignIn(usuario, usuario.IsRememberLogin);

                            //envia email de confirnação de cadastro
                            new SendMailController().ConfirmaCadastro(usuario).Deliver();
                            //authenticationService.SignIn(usuario, usuario.IsRememberLogin);

                            return this.RedirectToAction("CadastroSucesso");
                        }
                        else
                        {
                            foreach (var item in result)
                            {
                                TempData["MsgErro"] = item;
                            }
                        }
                    }
                    else
                    {
                        TempData["MsgErro"] = "A senha e confirma senha precisam ser idênticas";
                    }
                }
            }
            catch (Exception ex)
            {
                ObjLog.Error("ContaController(Cadastro): " + ex.ToString());
            }

            return View(usuario);
        }

        public ActionResult CadastroSucesso()
        {
            return View();
        }

        public ActionResult Login()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            if (usuario != null)
                return View(usuario);
            else
                return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            Session["init"] = null;

            var model = new Usuario();
            try
            {
                TryUpdateModel(model, collection);

                if (usuarioRepository.ValidateUsuario(model.Email, model.Senha))
                {
                    var usuario = usuarioRepository.GetEmail(model.Email);

                    if (usuario.IsAtivo)
                    {
                        authenticationService.SignIn(usuario, usuario.IsRememberLogin);

                        return RedirectToAction("Plantel", "Clube");
                    }
                    else
                    {
                        TempData["MsgErro"] = "Esse usuario está inativo no momento.";

                    }
                }
                else
                {
                    TempData["MsgErro"] = "e-mail ou senha incorreta.";
                }
            }
            catch (Exception ex)
            {
                ObjLog.Error("ContaController(Login): " + ex.ToString());
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult MenuTop()
        {
            var usuario = authenticationService.GetUserAuthenticated();
            
            if (usuario == null)
                return View();
            else
                return View(usuario);
        }

        public ActionResult Ativar(string id)
        {
            var usuario = usuarioRepository.GetGuid(id);

            try
            {
                if (usuario != null)
                {
                    usuario.Guid = null;
                    usuario.IsAtivo = true;

                    usuarioRepository.SaveOrUpdate(usuario);

                    var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Id;
                    var controle = controleRepository.GetAll().FirstOrDefault();

                    foreach (var clube in clubeRepository.GetAll().Where(x => x.Divisao.Id == ultdivisao && x.Usuario == null))
                    {
                        var usuariooferta = new UsuarioOferta();

                        usuariooferta.Clube = clube;
                        usuariooferta.Dia = controle.Dia;
                        usuariooferta.Usuario = usuario;
                        usuarioofertaRepository.SaveOrUpdate(usuariooferta);
                    }
                }
            }
            catch (Exception ex)
            {
                ObjLog.Error("ContaController(Ativar): " + ex.ToString());
            }

            return View(usuario);
        }

        public ActionResult RecuperarSenha()
        {
            return View();
        }

        [HttpPost]
        [Transaction]
        public ActionResult RecuperarSenha(String email)
        {
            var user = usuarioRepository.GetEmail(email);

            try
            {
                if (email == "")
                {
                    ModelState.AddModelError("Email", "Campo obrigatório.");
                    return View(user);
                }

                if (user != null)
                {
                    ViewBag.user = user.NomeCompleto;
                    Random r = new Random();
                    int pass = r.Next(100000, 999999);
                    user.Senha = FormsAuthentication.HashPasswordForStoringInConfigFile(pass.ToString(), "SHA1");

                    new SendMailController().PasswordReset(email, pass.ToString()).Deliver();

                    usuarioRepository.SaveOrUpdate(user);

                    //redirecionar para página que redireciona para pagina inicial inicial
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("email", "E-mail não encontrado.");
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ObjLog.Error(string.Format("ContaController(RecuperarSenha): {0}", ex.Message));
                TempData["MessageError"] = true;
                return View(user);
            }
        }

        [Authorize]
        public ActionResult Sair()
        {
            this.authenticationService.SignOut();
            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult UsuarioOferta()
        {
            var usuario = authenticationService.GetUserAuthenticated();

            var ofertas = usuarioofertaRepository.GetAll().Where(x => x.Usuario.Id == usuario.Id);

            if (ofertas.Count() > 0)
                return View(ofertas);
            else
                return View();
        }

        [Authorize]
        [Transaction]
        public ActionResult UsuarioOfertaResposta(int id, bool resposta)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var usuariooferta = usuarioofertaRepository.Get(id);

            if (resposta)
            {
                usuarioRepository.AceitaUsuarioOferta(usuario.Id, usuariooferta.Clube.Id);

                return RedirectToAction("Plantel", "Clube");
            }
            else
            {
                usuarioofertaRepository.Delete(usuariooferta);
                return RedirectToAction("UsuarioOferta");
            }            
        }
    }
}
