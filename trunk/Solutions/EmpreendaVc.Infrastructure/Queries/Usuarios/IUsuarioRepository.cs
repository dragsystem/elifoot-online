using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.NHibernate;
using EmpreendaVc.Domain;

namespace EmpreendaVc.Infrastructure.Queries.Usuarios
{
    public interface IUsuarioRepository
    {
        Usuario Get(int Id);
        Usuario GetEmail(string Email);
        Usuario GetNome(string Nome);
        Usuario GetGuid(string Guid);
        IList<Usuario> GetAll();
        IList<Usuario> Busca(string nome);

        List<string> SaveOrUpdate(Usuario userAccount);

        bool ValidateUsuario(string Email, string password);

        void AceitaUsuarioOferta(int id, int clube);
    }
}
