using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Nome")]
    public class Nome : Entity
    {
        public virtual string NomeJogador { get; set; }

        public virtual bool Comum { get; set; }
    }
}
