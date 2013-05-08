using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.NHibernate;
using EmpreendaVc.Domain;
using System.Web.Security;
using NHibernate.Criterion;

namespace EmpreendaVc.Infrastructure.Queries.Usuarios
{
    public class UsuarioRepository : NHibernateQuery<Usuario>, IUsuarioRepository
    {
        public override IList<Usuario> ExecuteQuery()
        {
            throw new NotImplementedException();
        }

        public Usuario Get(int Id)
        {
            return Session.QueryOver<Usuario>()
                .Where(x => x.Id == Id).SingleOrDefault();
        }

        public Usuario GetEmail(string Email)
        {
            return Session.QueryOver<Usuario>()
                .Where(x => x.Email == Email).SingleOrDefault();
        }

        public Usuario GetNome(string Nome)
        {
            return Session.QueryOver<Usuario>()
                .Where(x => x.NomeCompleto == Nome).SingleOrDefault();
        }

        public Usuario GetGuid(string Guid)
        {
            return Session.QueryOver<Usuario>()
                .Where(x => x.Guid == Guid).SingleOrDefault();
        }

        public IList<Usuario> GetAll()
        {
            return Session.QueryOver<Usuario>()
                .Where(x => x.IsAtivo).List();
        }

        public IList<Usuario> Busca(string nome)
        {
            return Session.QueryOver<Usuario>()
                .Where(x => x.IsAtivo)
                .Where(x => x.NomeCompleto.IsInsensitiveLike("%" + nome + "%") || x.Email.IsInsensitiveLike("%" + nome + "%")).List().ToList();
        }

        public List<string> SaveOrUpdate(Usuario userAccount)
        {
            var erros = new List<string>();

            if (userAccount.Id == 0)
            {
                erros = Save(userAccount);
            }
            else
            {
                erros = Update(userAccount);
            }

            return erros;
        }

        private List<string> Save(Usuario userAccount)
        {

            var erros = new List<string>();

            var email = userAccount.Email;

            //var userExist = Session.QueryOver<Usuario>().Where(x => x.Email == email).SingleOrDefault();

            //if (userExist == null)
            //{
                userAccount.Senha = UsuarioCommand.HasPasswordToString(userAccount.Senha);

                //salva usuário com pessoa vinculada
                Session.SaveOrUpdate(userAccount);

                //if (userAccount.Picks != null)
                //{
                //    foreach (var item in userAccount.Picks)
                //    {
                //        item.Usuario = userAccount;
                //        Session.Save(item);
                //    }
                //}
            //}
            //else
            //{
            //    erros.Add("E-mail já cadastrado");
            //}

            return erros;
        }

        private List<string> Update(Usuario userAccount)
        {

            Session.Transaction.Begin();

            var erros = new List<string>();

            try
            {
                //foreach (var item in userAccount.Picks)
                //{
                    

                //    if (item.Jogador != null && (item.Posicao != string.Empty && item.Posicao != null))
                //    {
                //        var repetido = Session.QueryOver<Pick>().Where(x => x.Usuario != item.Usuario && x.Jogador.Id == item.Jogador.Id).SingleOrDefault();
                //        if (repetido == null)
                //        {
                //            item.Usuario = item.Usuario == null ? userAccount : item.Usuario;
                //            Session.SaveOrUpdate(item);
                //        }
                //    }
                //}

                Session.SaveOrUpdate(userAccount);

                Session.Transaction.Commit();
            }
            catch (Exception)
            {
                erros.Add("Ocorreu erro ao salvar os dados do usuário");
                Session.Transaction.Rollback();
            }

            return erros;
        }

        public bool ValidateUsuario(string Email, string password)
        {
            Usuario userAccount = null;

            userAccount = GetEmail(Email);

            if (userAccount == null)
                return false;

            bool isValid = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1") == userAccount.Senha;

            return isValid;
        }
    }
}
