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

        public virtual int Contrato { get; set; }

        public virtual decimal Salario { get; set; }

        public virtual int Jogos { get; set; }

        public virtual decimal NotaUlt { get; set; }

        public virtual double NotaMedia { get { return (double)NotaTotal / Jogos; } }

        public virtual decimal NotaTotal { get; set; }

        public virtual int Treinos { get; set; }

        public virtual decimal TreinoUlt { get; set; }

        public virtual double TreinoMedia { get { return (double)TreinoTotal / Treinos; } }

        public virtual decimal TreinoTotal { get; set; }

        public virtual bool Temporario { get; set; }

        public virtual int Situacao { get; set; }
        //1 - Importante no clube
        //2 - Reserva
        //3 - Disponível para Transferência

        public virtual int Lesionado { get; set; }

        public virtual IList<Gol> Gols { get; set; }

        public virtual IList<JogadorOferta> JogadorOfertas { get; set; }

        public virtual IList<JogadorHistorico> Historico { get; set; }

        public Jogador()
        {
            Lesionado = 0;
            Situacao = 1;
            Jogos = 0;
            NotaTotal = 0;
            Gols = new List<Gol>();
            JogadorOfertas = new List<JogadorOferta>();
            Historico = new List<JogadorHistorico>();
        }
    }
}
