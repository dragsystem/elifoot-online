using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("LeilaoOferta")]
    public class LeilaoOferta : Entity
    {
        public virtual Leilao Leilao { get; set; }
                
        public virtual Clube Clube { get; set; }

        public virtual decimal Valor { get; set; }

        public LeilaoOferta()
        {
        }
    }
}
