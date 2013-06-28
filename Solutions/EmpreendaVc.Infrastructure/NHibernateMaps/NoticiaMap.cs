using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using EmpreendaVc.Domain;

namespace EmpreendaVc.Infrastructure.NHibernateMaps
{
    public class NoticiaMap : IAutoMappingOverride<Noticia>
    {
        public void Override(AutoMapping<Noticia> mapping)
        {
            mapping.Map(x => x.Texto).CustomType("StringClob").CustomSqlType("nvarchar(max)");
        }
    }
}
