using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.NHibernate;
using EmpreendaVc.Domain;
using System.Web.Security;
using NHibernate.Criterion;

namespace EmpreendaVc.Infrastructure.Queries.Clubes
{
    public class ClubeRepository : NHibernateQuery<Clube>, IClubeRepository
    {
        public override IList<Clube> ExecuteQuery()
        {
            throw new NotImplementedException();
        }

        public void ClassificadosTaca()
        {
            var itaca = 1;

            var lst = Session.QueryOver<DivisaoTabela>().List().OrderBy(x => x.Divisao.Numero).ThenBy(x => x.Posicao);
            foreach (var divisaotabela in lst)
            {
                var clube = Session.Get<Clube>(divisaotabela.Clube.Id);
                clube.Taca = true;

                Session.Save(clube);

                itaca++;
                if (itaca > 32)
                    break;
            }
        }

        public IList<Partida> PartidasClube(int id)
        {
            return Session.QueryOver<Partida>().Where(x => x.Clube1.Id == id || x.Clube2.Id == id).List();
        }

        public void TirarTreinador(int id)
        {
            var clube = Session.Get<Clube>(id);
            clube.Usuario = null;

            Session.Save(clube);
        }
    }
}
