using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace M17E_Caderneta.Data
{
    public class M17E_CadernetaContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public M17E_CadernetaContext() : base("name=M17E_CadernetaContext")
        {
        }

        public System.Data.Entity.DbSet<M17E_Caderneta.Models.Turma> Turmas { get; set; }
        public System.Data.Entity.DbSet<M17E_Caderneta.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<M17E_Caderneta.Models.Disciplina> Disciplinas { get; set; }

        public System.Data.Entity.DbSet<M17E_Caderneta.Models.Nota> Notas { get; set; }
    }
}
