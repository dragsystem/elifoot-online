using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Escalacao")]
    public class Escalacao : Entity
    {
        public virtual Clube Clube { get; set; }

        public virtual Jogador Jogador { get; set; }

        public virtual int Posicao { get; set; }

        public virtual int H { get { return Jogador.H; } }

        public Escalacao()
        {
        }
    }
}
