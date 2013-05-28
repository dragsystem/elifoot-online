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
        public virtual Divisao Divisao { get; set; }

        public virtual string Tipo { get; set; }

        public virtual int Mao { get; set; }

        public virtual int Dia { get; set; }

        public virtual int Rodada { get; set; }

        public virtual Clube Clube1 { get; set; }

        public virtual int Gol1 { get; set; }

        public virtual Clube Clube2 { get; set; }

        public virtual int Gol2 { get; set; }

        public virtual Clube Vencedor { get; set; }

        public virtual int Publico { get; set; }

        public virtual string Penalti { get; set; }

        public virtual bool Realizada { get; set; }

        public virtual IList<Gol> Gols { get; set; }

        public virtual IList<Gol> Gols1 { get { return Gols.Where(x => x.Clube.Id == Clube1.Id).OrderBy(x => x.Minuto).ToList(); } }

        public virtual IList<Gol> Gols2 { get { return Gols.Where(x => x.Clube.Id == Clube2.Id).OrderBy(x => x.Minuto).ToList(); } }

        public Partida()
        {
            Gols = new List<Gol>();
        }
    }
}
