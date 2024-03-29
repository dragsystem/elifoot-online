﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorOferta")]
    public class JogadorOferta : Entity
    {
        public virtual int Dia { get; set; }

        public virtual Jogador Jogador { get; set; }
                
        public virtual Clube Clube { get; set; }

        public virtual int Tipo { get; set; }
        //COMPRA: Tipo = 1 
        //ASSINAR: Tipo = 2
        //ASSINAR PRÉ CONTRATO: Tipo = 3

        public virtual int Estagio { get; set; }
        //COMPRA: Estagio: 1 - AGUARDANDO CLUBE RESPONDER / 2 - Aguardando resposta do jogador
        //2 - Aguardando resposta do jogador
        
        //3 - FINALIZADA
        // 0 - recusada

        public virtual decimal Valor { get; set; }

        public virtual decimal Salario { get; set; }

        public virtual int Contrato { get; set; }

        public virtual int Pontos { get; set; }

        public JogadorOferta()
        {
            Estagio = 1;
            Contrato = 2;
        }
    }
}
