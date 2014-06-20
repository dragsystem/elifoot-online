using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("JogadorPedidoLeilao")]
    public class JogadorPedidoLeilao : Entity
    {
        public virtual DateTime Dia { get; set; }

        public virtual Jogador Jogador { get; set; }

        public virtual decimal Valor { get; set; }

        public JogadorPedidoLeilao()
        {
            Dia = DateTime.Now;
        }
    }
}
