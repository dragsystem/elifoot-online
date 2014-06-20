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

        public virtual int Satisfacao { get; set; }
        //0 - Contente com sua situação no clube
        //1 - Indiferente
        //2 - Insatisfeito. Deseja sair do clube

        public virtual decimal Valor
        {
            get
            {
                if (Clube != null)
                {
                    if (Situacao == 1)
                        return (H * (H - 20 > 1 ? H - 20 : 1)) * 900 + (Posicao * 50000);
                    else if (Situacao == 2)
                        return (H * (H - 20 > 1 ? H - 20 : 1)) * 600 + (Posicao * 50000);
                    else
                        return (H * (H - 20 > 1 ? H - 20 : 1)) * 300 + (Posicao * 50000);
                }
                else
                    return 0;
            }
        }

        public virtual int Condicao { get; set; }

        public virtual int Lesionado { get; set; }

        public virtual IList<Gol> Gols { get; set; }

        public virtual IList<JogadorOferta> JogadorOfertas { get; set; }

        public virtual IList<JogadorPreContrato> JogadorPreContrato { get; set; }

        public virtual IList<JogadorHistorico> Historico { get; set; }

        public Jogador()
        {
            Condicao = 100;
            Lesionado = 0;
            Situacao = 1;
            Jogos = 0;
            NotaTotal = 0;
            Gols = new List<Gol>();
            JogadorOfertas = new List<JogadorOferta>();
            JogadorPreContrato = new List<JogadorPreContrato>();
            Historico = new List<JogadorHistorico>();
        }
    }
}
