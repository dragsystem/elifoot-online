using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using EmpreendaVc.Domain;

namespace EmpreendaVc.Infrastructure.NHibernateMaps
{
    public class ArtilheiroMap : IAutoMappingOverride<Artilheiro>
    {
        public void Override(AutoMapping<Artilheiro> mapping) {
            mapping.Table("vwArtilheiros");
            mapping.ReadOnly();
        }
    }
}
