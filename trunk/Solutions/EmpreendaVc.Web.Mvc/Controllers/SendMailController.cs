using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ActionMailer.Net.Mvc;
using System.Web.Mvc;
using EmpreendaVc.Domain;
using System.Web.Mail;

namespace EmpreendaVc.Web.Mvc.Controllers
{
    public class SendMailController : MailerBase
    {
        //
        // GET: /SendMail/

        //public EmailResult NossosVideos(FaleConosco obj)
        //{
        //    Subject = obj.Subject;
        //    From = obj.Mail;
        //    To.Add("info@empreenda.vc");
        //    ViewBag.NomeUsu = obj.Name;
        //    return Email("NossosVideos", obj);
        //}

        public EmailResult Contato(Contato obj)
        {
            Subject = obj.Assunto;
            From = "contato@desafioufc.com";
            To.Add("contato@desafioufc.com");

            return Email("Contato", obj);
        }

        //public EmailResult ReportarErro(FaleConosco obj)
        //{
        //    Subject = "Reportar Error";
        //    From = obj.Mail;
        //    To.Add("info@empreenda.vc");
        //    ViewBag.NomeUsu = obj.Name;
        //    return Email("ReportarErro", obj);
        //}

        //public EmailResult UsuMensagemMail(Usuario De, Usuario Para, UsuarioMensagem UsuarioMensagemDe, UsuarioMensagem UsuarioMensagemPara)
        //{
        //    Subject = "empreenda.vc - Mensagem";
        //    From = "info@empreenda.vc";
        //    To.Add(Para.UserName);
        //    ViewBag.NomeUsu = De.Person.NomePublico;
        //    ViewBag.NomeDestino = Para.Person.NomePublico;
        //    return Email("UsuMensagemMail", UsuarioMensagemDe);
        //}

        public EmailResult ConfirmaCadastro(Usuario obj)
        {
            Subject = "Confirmação de cadastro";
            From = "contato@desafioufc.com";
            To.Add(obj.Email);

            return Email("ConfirmaCadastro", obj);
        }

        //public EmailResult ConviteInvestidor(Usuario obj, Project proj, List<Domain.Professionals.Experience> Exp)
        //{
        //    Subject = "empreenda.vc - Convite de Projeto";
        //    From = "info@empreenda.vc";
        //    To.Add(obj.UserName);
        //    ViewBag.Usuario = obj;
        //    ViewBag.NomeUsu = obj.Person.NomePublico;
        //    //ViewBag.TipoConexao = tipoConexao;
        //    return Email("ConviteInvestidor", proj);
        //}

        //public EmailResult ConviteParticipante(Usuario Para, Project proj, List<Domain.Professionals.Experience> Exp)
        //{


        //    Subject = "empreenda.vc - Convite de Projeto";
        //    From = "info@empreenda.vc";
        //    To.Add(Para.UserName);
        //    ViewBag.Usuario = Para;
        //    ViewBag.NomeUsu = Para.Person.NomePublico;

        //    //ViewBag.TipoConexao = tipoConexao;
        //    //ViewBag.Frase = frase;
        //    return Email("ConviteParticipante", proj);
        //}

        //public EmailResult ConviteParticipanteExterno(ConviteExterno Para, Project proj)
        //{


        //    Subject = "empreenda.vc - Convite de Projeto";
        //    From = "info@empreenda.vc";
        //    To.Add(Para.Email);
        //    ViewBag.Usuario = Para.Email;
        //    ViewBag.NomeUsu = Para.Email;

        //    return Email("ConviteParticipanteExterno", proj);
        //}

        //public EmailResult ConectarUsuario(Usuario De, Usuario Para, List<Domain.Professionals.Experience> Exp)
        ////public EmailResult ConectarUsuario(Usuario De, Usuario Para)
        //{
        //    Subject = "empreenda.vc - Convite para Conexão";
        //    From = "info@empreenda.vc";
        //    To.Add(Para.UserName);
        //    ViewBag.NomeUsu = Para.Person.NomePublico;
        //    ViewBag.Solicitante = De.Person.NomePublico;

        //    var CargoAtual = Exp.FirstOrDefault(x => x.IsEmpregoAtual);

        //    if (CargoAtual != null)
        //    {
        //        ViewBag.CargoAtual = CargoAtual.Cargo;
        //        ViewBag.EmpresaAtual = CargoAtual.Empresa;
        //    }
        //    else
        //    {
        //        ViewBag.CargoAtual = "Nenhum";
        //        ViewBag.EmpresaAtual = "Nenhuma";
        //    }

        //    return Email("ConectarUsuario", De);

        //}

        //public EmailResult QueroInvestir(Usuario De, Usuario Para, Project proj, List<Domain.Professionals.Experience> Exp)
        //{
        //    Subject = "empreenda.vc - Investir";
        //    From = "info@empreenda.vc";
        //    To.Add(Para.UserName);
        //    ViewBag.Person_Nome = De.Person.NomePublico;
        //    ViewBag.NomeUsu = Para.Person.NomePublico;

        //    var CargoAtual = Exp.FirstOrDefault(x => x.IsEmpregoAtual);

        //    if (CargoAtual != null)
        //    {
        //        ViewBag.CargoAtual = CargoAtual.Cargo;
        //        ViewBag.EmpresaAtual = CargoAtual.Empresa;
        //    }
        //    else
        //    {
        //        ViewBag.CargoAtual = "Nenhum";
        //        ViewBag.EmpresaAtual = "Nenhuma";
        //    }

        //    return Email("QueroInvestir", proj);

        //}

        //public EmailResult QueroParticipar(Usuario De, Usuario Para, Project proj, string ConnectionType, List<Domain.Professionals.Experience> Exp)
        //{
        //    Subject = "empreenda.vc - Participar";
        //    From = "info@empreenda.vc";
        //    To.Add(Para.UserName);
        //    ViewBag.Person_Nome = De.Person.NomePublico;
        //    ViewBag.ConnectionType = ConnectionType;
        //    ViewBag.NomeUsu = Para.Person.NomePublico;

        //    var CargoAtual = Exp.FirstOrDefault(x => x.IsEmpregoAtual);

        //    if (CargoAtual != null)
        //    {
        //        ViewBag.CargoAtual = CargoAtual.Cargo;
        //        ViewBag.EmpresaAtual = CargoAtual.Empresa;
        //    }
        //    else
        //    {
        //        ViewBag.CargoAtual = "Nenhum";
        //        ViewBag.EmpresaAtual = "Nenhuma";
        //    }

        //    return Email("QueroParticipar", proj);

        //}

        public EmailResult PasswordReset(string email, string senha)
        {
            ViewBag.Password = senha;
            ViewBag.Data = string.Format("{0:dd/MM/yyyy}", DateTime.Now);

            Subject = "Renovação de senha";
            From = "contato@desafioufc.com";
            ViewBag.NomeUsu = email;
            To.Add(email);

            return Email("PasswordReset");
        }
    }
}
