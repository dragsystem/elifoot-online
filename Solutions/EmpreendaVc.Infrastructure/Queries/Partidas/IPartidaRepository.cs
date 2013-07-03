using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.NHibernate;
using EmpreendaVc.Domain;

namespace EmpreendaVc.Infrastructure.Queries.Partidas
{
    public interface IPartidaRepository
    {
        Partida Get(int Id);
        IList<Partida> GetAll();

        void LimparGols(int idpartida);

        void Delete(Partida partida);

        List<string> SaveOrUpdate(Partida userAccount);
    }
}
