using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Leilão")]
    public class Leilao : Entity
    {
        public virtual Jogador Jogador { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual decimal Valor { get; set; }

        public virtual bool Espontaneo { get; set; }

        public virtual int Dia { get; set; }

        public virtual LeilaoOferta OfertaVencedora { get; set; }

        public virtual IList<LeilaoOferta> Ofertas { get; set; }

        public Leilao()
        {
            Clube = Jogador.Clube;
            Ofertas = new List<LeilaoOferta>();
        }
    }
}
