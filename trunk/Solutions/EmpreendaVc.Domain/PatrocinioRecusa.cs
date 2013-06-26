using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("PatrocinioRecusa")]
    public class PatrocinioRecusa : Entity
    {
        public virtual Patrocinio Patrocinio { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual int Ano { get; set; }
    }
}
