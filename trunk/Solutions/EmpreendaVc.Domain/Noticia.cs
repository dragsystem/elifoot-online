using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Noticia")]
    public class Noticia : Entity
    {
        public virtual int Dia { get; set; }

        public virtual int Tipo { get; set; }
        //1 - Compra/Venda
        //2 - PedidoJogador
        //3 - OfertaUsuario
        //4 - Infos Titulo/Rebaixamento
        //5 - Demissão
        //6 - Jogador foi a leilao

        public virtual Jogador Jogador { get; set; }

        public virtual Clube ClubeComprador { get; set; }

        public virtual Clube ClubeVendedor { get; set; }

        public virtual decimal Valor { get; set; }

        public virtual decimal Salario { get; set; }

        public virtual JogadorPedido JogadorPedido { get; set; }

        public virtual UsuarioOferta UsuarioOferta { get; set; }

        public Noticia()
        {
        }   
    }
}
