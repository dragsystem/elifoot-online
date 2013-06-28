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
        //3-2-2-3
        //3-1-3-3
        //3-3-1-3
        //3-3-2-2
        //3-2-3-2
        //3-3-3-1
        //4-2-1-3
        //4-1-2-3
        //4-2-2-2
        //4-3-1-2
        //4-1-3-2
        //4-3-2-1
        //4-2-3-1
        //5-1-1-3
        //5-2-1-2
        //5-1-2-2
        //5-2-2-1
        //5-3-1-1
        //5-1-3-1
        //6-2-1-1
        //6-1-2-1
        //6-1-1-2

        public virtual bool Taca { get; set; }

        public virtual int ReputacaoAI { get; set; }

        public virtual int Socios { get; set; }

        public virtual decimal Ingresso { get; set; }

        public virtual IList<Jogador> Jogadores { get; set; }

        public virtual IList<Escalacao> Escalacao { get; set; }

        public virtual IList<Partida> Partidas { get; set; }

        public virtual IList<DivisaoTabela> DivisaoTabelas { get; set; }

        public virtual IList<JogadorOferta> Ofertas { get; set; }

        public virtual IList<PatrocinioClube> PatrocinioClubes { get; set; }

        public virtual IList<PatrocinioRecusa> PatrocinioRecusas { get; set; }

        public Clube()
        {
            Socios = 1000;
            Ingresso = 20;
            ReputacaoAI = 50;
            Jogadores = new List<Jogador>();
            Escalacao = new List<Escalacao>();
            Partidas = new List<Partida>();
            DivisaoTabelas = new List<DivisaoTabela>();
            Ofertas = new List<JogadorOferta>();
            PatrocinioClubes = new List<PatrocinioClube>();
            PatrocinioRecusas = new List<PatrocinioRecusa>();
            Formacao = "4222";
        }
    }
}
