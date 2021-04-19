using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace M17E_Caderneta.Models
{
    public class Turma
    {
        [Key]
        public int id { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage ="Tem de indicar o ano letivo")]
        [Display(Name ="Ano Letivo")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        public DateTime AnoLetivo { get; set; }

        [Required(ErrorMessage = "Indique o Ano da turma")]
        [Range(7,12,ErrorMessage = "O ano deve ser entre o 7º e o 12º Ano")]
        public int Ano { get; set; }

        
        private string letra;
        [Required(ErrorMessage = "Indique a letra da turma")]
        [StringLength(2)]
        [UIHint("Insira uma letra pelos menos.")]
        public string Letra
        {
            get { return letra; }
            set { letra = value.ToUpper(); }
        }

        [NotMapped]
        public string Nome
        {
            get
            {
                return string.Format($"{Ano}{Letra} ({AnoLetivo.Year.ToString()}-{AnoLetivo.AddYears(1).Year.ToString()})");
            }
        }
    }
}