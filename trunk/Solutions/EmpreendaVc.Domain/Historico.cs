using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Historico")]
    public class Historico : Entity
    {
        public virtual int Ano { get; set; }
                
        public virtual Divisao Divisao { get; set; }

        public virtual Jogador Artilheiro { get; set; }

        public virtual bool Taca { get; set; }

        public virtual int Gols { get; set; }

        public virtual Clube Campeao { get; set; }

        public virtual Clube Vice { get; set; }
        
        public Historico()
        {
        }
    }
}
