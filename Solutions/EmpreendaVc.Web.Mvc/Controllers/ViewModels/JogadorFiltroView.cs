using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmpreendaVc.Web.Mvc.Controllers.ViewModels
{
    public class JogadorFiltroView
    {        
        public string Nome { get; set; }
        public int Posicao { get; set; }
        public int Situacao { get; set; }
        public int Contrato { get; set; }
        public int Ordenacao { get; set; }
    }
}