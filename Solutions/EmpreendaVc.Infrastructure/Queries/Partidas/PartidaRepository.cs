using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.NHibernate;
using EmpreendaVc.Domain;
using System.Web.Security;
using NHibernate.Criterion;

namespace EmpreendaVc.Infrastructure.Queries.Partidas
{
    public class PartidaRepository : NHibernateQuery<Partida>, IPartidaRepository
    {
        public override IList<Partida> ExecuteQuery()
        {
            throw new NotImplementedException();
        }

        public Partida Get(int Id)
        {
            return Session.QueryOver<Partida>()
                .Where(x => x.Id == Id).SingleOrDefault();
        }      

        public IList<Partida> GetAll()
        {
            return Session.QueryOver<Partida>().List();
        }

        public List<string> SaveOrUpdate(Partida partida)
        {
            var erros = new List<string>();

            if (partida.Id == 0)
            {
                erros = Save(partida);
            }
            else
            {
                erros = Update(partida);
            }

            return erros;
        }

        private List<string> Save(Partida partida)
        {

            var erros = new List<string>();

            Session.SaveOrUpdate(partida);

            return erros;
        }

        private List<string> Update(Partida partida)
        {

            Session.Transaction.Begin();

            var erros = new List<string>();

            try
            {
                Session.SaveOrUpdate(partida);

                Session.Transaction.Commit();
            }
            catch (Exception)
            {
                erros.Add("Ocorreu erro ao salvar os dados da partida");
                Session.Transaction.Rollback();
            }

            return erros;
        }

        public void Delete(Partida partida)
        {
            Session.Delete(partida);
        }

        public void LimparGols(int idpartida)
        {
            var lst = Session.QueryOver<Gol>().Where(x => x.Partida.Id == idpartida).List();

            foreach (var item in lst)
            {
                Session.Delete(item);
            }

            Session.Transaction.Commit();
        }
    }
}
