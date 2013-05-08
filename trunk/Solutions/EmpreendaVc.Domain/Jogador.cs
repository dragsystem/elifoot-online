using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Jogador")]
    public class Jogador : Entity
    {
        public virtual string Nome { get; set; }
                
        public virtual int Posicao { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual int H { get; set; }

        public virtual int HF { get; set; }

        public virtual bool Contrato { get; set; }

        public virtual decimal Salario { get; set; }

        public virtual IList<Gol> Gols { get; set; }
        public Jogador()
        {
            Gols = new List<Gol>();
        }
    }
}
