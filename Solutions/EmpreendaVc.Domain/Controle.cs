using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Controle")]
    public class Controle : Entity
    {
        public virtual DateTime Data { get; set; }
                
        public virtual int Ano { get; set; }

        public virtual int Rodada { get; set; }

        public virtual int Taca { get; set; }

        public Controle()
        {
            Data = DateTime.Now;
        }   
    }
}
