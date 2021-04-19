using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace M17E_Caderneta.Models
{
    public class Nota
    {
        [Key]
        public int Id { get; set; }

        
        [Required(ErrorMessage = "Tem de indicar um aluno")]
        [Display(Name = "Aluno")]
        [ForeignKey("Aluno")]
        public int IdAluno { get; set; }
        public virtual User Aluno { get; set; }

        
        [Required(ErrorMessage = "Tem de indicar um professor")]
        [Display(Name = "Professor")]
        [ForeignKey("Professor")]
        public int IdProfessor { get; set; }
        public virtual User Professor { get; set; }


        
        [Required(ErrorMessage = "Tem de indicar uma disciplina")]
        [Display(Name = "Disciplina")]
        [ForeignKey("Disciplina")]
        public int IdDisciplina { get; set; }
        public virtual Disciplina Disciplina { get; set; }

        [Display(Name =  "Data de criação")]
        public DateTime CreateDate { get; set; }


        [Required(ErrorMessage = "Tem de indicar uma disciplina")]
        public int Valor { get; set; }
        public Nota()
        {
            CreateDate = DateTime.Now;
        }
    }
}