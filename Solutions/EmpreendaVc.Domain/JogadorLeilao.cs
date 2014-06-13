using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorLeilao")]
    public class JogadorLeilao : Entity
    {
        public virtual DateTime Dia { get; set; }

        public virtual Jogador Jogador { get; set; }
                
        public virtual Clube Clube { get; set; }

        public virtual int Estagio { get; set; }
        //COMPRA: Estagio: 1 - AGUARDANDO CLUBE RESPONDER / 2 - Aguardando resposta do jogador
        //2 - Aguardando resposta do jogador
        
        //3 - FINALIZADA
        // 0 - recusada

        public virtual decimal Valor { get; set; }

        public JogadorLeilao()
        {
            Estagio = 1;
        }
    }
}
