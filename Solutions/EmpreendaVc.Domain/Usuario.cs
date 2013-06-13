using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpArch.Domain.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace EmpreendaVc.Domain
{
    [DisplayName("Usuário")]
    public class Usuario : Entity
    {
        [Required]
        public virtual string NomeCompleto { get; set; }

        [Required]
        [DisplayName("Senha")]
        public virtual string Senha { get; set; }

        [Required]
        [DisplayName("Email")]
        public virtual string Email { get; set; }

        [DisplayName("continuar conectado")]
        public virtual bool IsRememberLogin { get; set; }

        public virtual bool IsAtivo { get; set; }

        public virtual string IP { get; set; }

        public virtual DateTime DataCriado { get; set; }
        public virtual DateTime DataLogin { get; set; }

        public virtual string Guid { get; set; }

        public virtual Clube Clube { get; set; }

        public virtual int Reputacao { get; set; }

        public virtual int ReputacaoGeral { get; set; }

        public virtual int DelayTroca { get; set; }

        public virtual int IdUltimoClube { get; set; }

        public virtual IList<Noticia> Noticias { get; set; }        

        public Usuario()
        {
            DataCriado = DateTime.Now;
            DataLogin = DateTime.Now;
            Noticias = new List<Noticia>();
            Reputacao = 30;
            ReputacaoGeral = 0;
        }
    }
}
