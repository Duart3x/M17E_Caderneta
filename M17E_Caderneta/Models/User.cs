using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace M17E_Caderneta.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Insira um email válido")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage ="Por favor indique um nome")]
        [MinLength(3,ErrorMessage ="Nome demasiado pequeno")]
        [MaxLength(50, ErrorMessage = "Nome demasiado grande")]

        public string Nome { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1/1/1940", "31/12/2011",
    ErrorMessage = "O valor {0:dd/MM/yyyy} deve estar entre {1:dd/MM/yyyy} e {2:dd/MM/yyyy}!")]
        public DateTime DataNascimento { get; set; }

        [Display(Name = "Número Interno")]
        [Required(ErrorMessage = "Insira o seu número interno (a123 ou p123)")]
        [RegularExpression(@"^[p,a][0-9]+$", ErrorMessage = "O número interno deve ser começado por uma letra (p,a)")]
        public string NumInterno { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Palavra passe")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Indique o perfil")]
        [Display(Name = "Perfil do utilizador")]
        public int Perfil { get; set; }

        [ForeignKey("Turma")]
        public int? TurmaId { get; set; }
        public Turma Turma { get; set; }

        [Display(Name = "Número Turma")]
        [Range(1,40, ErrorMessage = "Número inválido")]
        public int? NumTurma { get; set; }

        [Display(Name = "Estado da conta")]
        public bool estado { get; set; }

        public string lnkRecuperar { get; set; }

        [NotMapped]
        public string NomeCompleto { get {
            if(Perfil == 2)
            {
                return "("+NumInterno+") - ["+NumTurma+"] " + Nome + " {" + Turma.Nome+"}";
            }
            return "(" + NumInterno + ") " + Nome;
        }}

        public IEnumerable<System.Web.Mvc.SelectListItem> perfis { get; set; }
    }
}