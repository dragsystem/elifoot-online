using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorPedido")]
    public class Partida : Entity
    {
        public virtual Jogador Jogador { get; set; }

        public virtual string Tipo { get; set; }

        public virtual int Mao { get; set; }

        public virtual int Rodada { get; set; }

        public virtual Clube Clube1 { get; set; }

        public virtual int Gol1 { get; set; }

        public virtual Clube Clube2 { get; set; }

        public virtual int Gol2 { get; set; }

        public virtual Clube Vencedor { get; set; }

        public virtual int Publico { get; set; }

        public virtual int Penalti { get; set; }

        public virtual bool Realizada { get; set; }

        public Partida()
        {
        }
    }
}
