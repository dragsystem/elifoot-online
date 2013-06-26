using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("PatrocinioClube")]
    public class PatrocinioClube : Entity
    {
        public virtual Patrocinio Patrocinio { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual decimal Valor { get; set; }

        public virtual int Contrato { get; set; }

        public virtual int Tipo { get; set; }
        //1 - Master
        //2 - Manga
        //3 - Fornecedor
    }
}
