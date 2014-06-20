using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorLeilaoOferta")]
    public class JogadorLeilaoOferta : Entity
    {
        public virtual JogadorLeilao JogadorLeilao { get; set; }
                
        public virtual Clube Clube { get; set; }

        public virtual int Estagio { get; set; }

        public virtual decimal Salario { get; set; }

        public virtual int Contrato { get; set; }

        public virtual int Pontos { get; set; }

        public JogadorLeilaoOferta()
        {
        }
    }
}
