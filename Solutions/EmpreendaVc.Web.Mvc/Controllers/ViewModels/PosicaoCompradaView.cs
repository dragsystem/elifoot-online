using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmpreendaVc.Domain;

namespace EmpreendaVc.Web.Mvc.Controllers.ViewModels
{
    public class PosicaoCompradaView
    {
        public int Posicao { get; set; }
        public Clube Clube { get; set; }
    }
}