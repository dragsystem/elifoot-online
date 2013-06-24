using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorTeste")]
    public class JogadorTeste : Entity
    {
        public virtual Jogador Jogador { get; set; }
        
        public virtual Clube Clube { get; set; }

        public virtual int Dia { get; set; }

        public JogadorTeste()
        {
        }
    }
}
