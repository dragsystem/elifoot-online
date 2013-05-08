using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Gol")]
    public class Gol : Entity
    {
        public virtual Partida Partida { get; set; }

        public virtual Jogador Jogador { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual int Minuto { get; set; }

        public Gol()
        {
        
        }
    }
}
