using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Clube")]
    public class Clube : Entity
    {
        [Required]
        public virtual string Nome { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual decimal Dinheiro { get; set; }

        public virtual int Estadio { get; set; }

        public virtual Divisao Divisao { get; set; }

        public virtual string Formacao { get; set; }

        public virtual bool Taca { get; set; }

        public virtual int ReputacaoAI { get; set; }

        public virtual int Socios { get; set; }

        public virtual decimal Ingresso { get; set; }

        public virtual IList<Jogador> Jogadores { get; set; }

        public virtual IList<Escalacao> Escalacao { get; set; }

        public virtual IList<Partida> Partidas { get; set; }

        public virtual IList<LeilaoOferta> Ofertas { get; set; }

        public Clube()
        {
            Socios = 1000;
            Ingresso = 20;
            ReputacaoAI = 50;
            Jogadores = new List<Jogador>();
            Escalacao = new List<Escalacao>();
            Partidas = new List<Partida>();
            Ofertas = new List<LeilaoOferta>();
        }
    }
}
