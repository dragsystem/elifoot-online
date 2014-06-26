using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmpreendaVc.Web.Mvc.Controllers.ViewModels
{
    public class PartidaView
    {
        public int PartidaId { get; set; }
        public int Clube1Id { get; set; }
        public int Clube1Gol { get; set; }
        public string Clube1Nome { get; set; }
        public int Clube2Id { get; set; }
        public string Clube2Nome { get; set; }
        public int Clube2Gol { get; set; }
        public int Vencedor { get; set; }
        public int Publico { get; set; }
        public IList<GolView> Gols { get; set; }

        public PartidaView()
        {
            Gols = new List<GolView>();
        }

    }
}