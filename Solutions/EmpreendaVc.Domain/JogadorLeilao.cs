using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorLeilao")]
    public class JogadorLeilao : Entity
    {
        public virtual int Dia { get; set; }

        public virtual Jogador Jogador { get; set; }
                
        public virtual Clube Clube { get; set; }

        public virtual int Estagio { get; set; }
        //Ofertas: Estagio: 1 
        //2 - FINALIZADA

        public virtual decimal Valor { get; set; }

        public virtual bool Espontaneo { get; set; }

        public virtual IList<JogadorLeilaoOferta> LeilaoOfertas { get; set; }
        
        public virtual Clube Vencedor { get; set; }

        public JogadorLeilao()
        {
            Estagio = 1;
        }
    }
}
