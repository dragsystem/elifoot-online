using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("UsuarioOferta")]
    public class UsuarioOferta : Entity
    {
        public virtual Usuario Usuario { get; set; }
                
        public virtual Clube Clube { get; set; }

        public virtual int Rodada { get; set; }

        public UsuarioOferta()
        {
        }   
    }
}
