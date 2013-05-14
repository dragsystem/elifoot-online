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

        public virtual int HGol
        {
            get
            {
                if (Jogador.Posicao == 7)
                    return Jogador.H;
                else if (Jogador.Posicao == 6)
                    return (Jogador.H * 5)/10;
                else if (Jogador.Posicao == 1)
                    return 1;
                else
                    return Jogador.H/10;
            }
        }

        public Escalacao()
        {
        }
    }
}
