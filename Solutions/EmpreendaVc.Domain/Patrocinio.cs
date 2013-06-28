using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Patrocinio")]
    public class Patrocinio : Entity
    {
        public virtual string Nome { get; set; }
                
        public virtual decimal ValorMax { get; set; }

        public virtual int DivisaoMinima { get; set; }

        public virtual int Tipo { get; set; }
        //1 - Mídia
        //2 - Fornecedor

        public virtual IList<PatrocinioClube> PatrocinioClubes { get; set; }
        public virtual IList<PatrocinioRecusa> PatrocinioRecusas { get; set; }
    }
}
