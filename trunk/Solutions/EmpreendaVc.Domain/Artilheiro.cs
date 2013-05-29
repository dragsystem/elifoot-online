using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Artilheiro")]
    public class Artilheiro : Entity
    {
        [Required]
        public virtual Jogador Jogador { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual int Divisao { get; set; }

        public virtual int Taca { get; set; }
    }
}
