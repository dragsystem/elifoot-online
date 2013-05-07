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

        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime EditDate { get; set; }

        public virtual IList<Grupo> AdmGrupos { get; set; }

        public virtual IList<UsuarioGrupo> Grupos { get; set; }

        public virtual IList<Convite> Convites { get; set; }

        public virtual IList<Jogo> Jogos { get; set; }

        public virtual string Guid { get; set; }

        public Usuario()
        {
            CreatedDate = DateTime.Now;
            EditDate = DateTime.Now;
        }
    }
}
