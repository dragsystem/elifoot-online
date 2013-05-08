using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using EmpreendaVc.Domain.People;
using EmpreendaVc.Domain.Professionals;

namespace EmpreendaVc.Infrastructure.NHibernateMaps
{
    public class UserAccountMap : IAutoMappingOverride<UserAccount>
    {
        public void Override(AutoMapping<UserAccount> mapping) {
            mapping.IgnoreProperty(x => x.IsRememberLogin);
            mapping.IgnoreProperty(x => x.ConfirmPassword);
            mapping.IgnoreProperty(x => x.Experiences);
            mapping.IgnoreProperty(x => x.IsConsultor);
            mapping.IgnoreProperty(x => x.ExperiencesAtuais);
            mapping.IgnoreProperty(x => x.FotoDoUsuario);
            mapping.HasManyToMany<Specialty>(x => x.Specialties).Table("SpecialtiesToUserAccounts");
            mapping.HasManyToMany<SegmentCompany>(x => x.SegmentCompanys).Table("SegmentCompanysToUserAccounts");
            mapping.NaturalId().Property(x => x.UserName);
        }
    }
}
