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

        public virtual Usuario Usuario { get; set; }

        public virtual string Texto { get; set; }

        public virtual DateTime Data { get; set; }

        public Noticia()
        {
            Data = DateTime.Now;
        }   
    }
}
