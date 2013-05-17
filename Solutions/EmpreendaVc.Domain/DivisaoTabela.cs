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
    public class DivisaoTabela : Entity
    {
        public virtual Divisao Divisao { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual int Pontos { get; set; }

        public virtual int GP { get; set; }

        public virtual int GC { get; set; }

        public virtual int J { get; set; }

        public virtual int V { get; set; }

        public virtual int E { get; set; }

        public virtual int D { get; set; }

        public virtual int Saldo { get { return (GP - GC); } }

        public virtual int Posicao { get; set; }

        public virtual decimal Aproveitamento { get { return Pontos > 0 ? (Pontos / (J * 3)) * 100 : 0; } }

        public DivisaoTabela()
        {
            GP = 0;
            GC = 0;
            J = 0;
            V = 0;
            E = 0;
            D = 0;
        }
    }
}
