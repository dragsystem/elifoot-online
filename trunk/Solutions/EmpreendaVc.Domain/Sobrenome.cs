using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Sobrenome")]
    public class Sobrenome : Entity
    {
        public virtual string SobrenomeJogador { get; set; }
    }
}
