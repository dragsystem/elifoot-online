using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorHistorico")]
    public class JogadorHistorico : Entity
    {
        public virtual int Ano { get; set; }

        public virtual Jogador Jogador { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual int Jogos { get; set; }

        public virtual int Gols { get; set; }

        public virtual double NotaMedia { get; set; }

        public virtual decimal Valor { get; set; }

        public virtual DateTime Data { get; set; }

        public JogadorHistorico()
        {
            Data = DateTime.Now;
        }
    }
}
