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
        public virtual Usuario Usuario { get; set; }

        public virtual int Rodada { get; set; }

        public virtual string Texto { get; set; }

        public Noticia()
        {
        }   
    }
}
