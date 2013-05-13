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

        public virtual int H 
        { 
            get 
            {
                if (Jogador.H >= 90)
                    return Jogador.H + 20;
                else if (Jogador.H >= 80)
                    return Jogador.H + 10;
                else if (Jogador.H >= 70)
                    return Jogador.H + 5;
                else
                    return Jogador.H;
            } 
        }

        public Escalacao()
        {
        }
    }
}
