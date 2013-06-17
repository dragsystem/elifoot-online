using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Staff")]
    public class Staff : Entity
    {
        public virtual string Nome { get; set; }
                
        public virtual int Tipo { get; set; }
        //1 - Olheiro
        //2 - Médico

        public virtual Usuario Usuario { get; set; }

        public virtual int H { get; set; }

        public virtual int Contrato { get; set; }

        public virtual decimal Salario { get; set; }
        
        public Staff()
        {
            Contrato = 0;
            Salario = 0;
        }
    }
}
