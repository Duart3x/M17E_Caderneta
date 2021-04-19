using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace M17E_Caderneta.Models
{
    public class Disciplina
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Insira o nome da disciplina")]
        [MinLength(5, ErrorMessage = "Nome demasiado pequeno")]
        [MaxLength(100, ErrorMessage = "Nome demasiado grande")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Insira uma descrição para a disciplina")]
        [MinLength(5, ErrorMessage ="Descrição demasiado pequena")]
        [MaxLength(100,ErrorMessage ="Descrição demasiado grande")]
        public string Descricao { get; set; }
    }
}