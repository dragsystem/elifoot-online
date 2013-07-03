using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorPreContrato")]
    public class JogadorPreContrato : Entity
    {
        public virtual int Dia { get; set; }

        public virtual Jogador Jogador { get; set; }
                
        public virtual Clube Clube { get; set; }

        public virtual decimal Valor { get; set; }

        public virtual decimal Salario { get; set; }

        public virtual int Contrato { get; set; }

        public JogadorPreContrato()
        {
        }
    }
}
