using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.NHibernate;
using EmpreendaVc.Domain;

namespace EmpreendaVc.Infrastructure.Queries.Clubes
{
    public interface IClubeRepository
    {
        IList<Partida> PartidasClube(int id);
        void ClassificadosTaca();
    }
}
