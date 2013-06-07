using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmpreendaVc.Web.Mvc.Controllers.ViewModels
{
    public class EscalacaoView
    {
        public int Id { get; set; }
        public int JogadorId { get; set; }
        public string Posicao { get; set; }
    }
}