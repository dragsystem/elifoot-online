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

        public ContaController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
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
                    ModelState.AddModelError("", "Nome já em uso");
            }
            else
                ModelState.AddModelError("", "Nome é um campo obrigatório");

            if (usuario.Email != null)
            {
                if (usuarioRepository.GetEmail(usuario.Email) != null)
                    ModelState.AddModelError("", "E-mail já em uso");

                if (usuario.Email.Contains("@@"))
                    ModelState.AddModelError("", "Favor preencher um E-mail válido");
            }
            else
                ModelState.AddModelError("", "E-mail é um campo obrigatório");

            if (usuario.Senha != null)
            {
                if (usuario.Senha.Length < 6)
                    ModelState.AddModelError("", "A senha precisa ter 6 ou mais caracteres");
            }
            else
            {
                ModelState.AddModelError("", "Favor preencher a senha");
            }

            var ConfirmaSenha = collection["ConfirmaSenha"].ToString();

            if (ConfirmaSenha == string.Empty)
                ModelState.AddModelError("", "Favor preencher a confirmação da senha");

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
                                ModelState.AddModelError("", item);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "A senha e confirma senha precisam ser idênticas");
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

                        return RedirectToAction("MeusGrupos", "Grupo");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Esse usuario está inativo no momento.");

                    }
                }
                else
                {
                    ModelState.AddModelError("", "e-mail ou senha incorreta.");
                }
            }
            catch (Exception ex)
            {
                ObjLog.Error("ContaController(Login): " + ex.ToString());
            }

            return View(model);
        }

        public ActionResult MenuTop()
        {
            var usuario = authenticationService.GetUserAuthenticated();


            if (usuario == null)
                return View("_MenuTop");
            else
                return View("_MenuTop", usuario);
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
    }
}
