using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Contato")]
    public class Contato : Entity
    {
        public virtual Usuario Usuario { get; set; }

        public virtual string Nome { get; set; }

        public virtual string Email { get; set; }

        [Required]
        public virtual string Assunto { get; set; }

        [Required]
        public virtual string Mensagem { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public Contato()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
