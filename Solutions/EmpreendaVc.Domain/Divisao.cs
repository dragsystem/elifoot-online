using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Divisao")]
    public class Divisao : Entity
    {
        public virtual int Numero { get; set; }

        public virtual string Nome { get { return Numero.ToString() + "ª Divisão"; } }

        public virtual bool Ativa { get; set; }

        public virtual IList<Clube> Clubes { get; set; }

        public Divisao()
        {
            Clubes = new List<Clube>();
        }
    }
}
