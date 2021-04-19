namespace M17E_Caderneta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseDados : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Disciplinas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdAluno = c.Int(nullable: false),
                        IdProfessor = c.Int(nullable: false),
                        IdDisciplina = c.Int(nullable: false),
                        Valor = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.IdAluno, cascadeDelete: true)
                .ForeignKey("dbo.Disciplinas", t => t.IdDisciplina, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.IdProfessor, cascadeDelete: false)
                .Index(t => t.IdAluno)
                .Index(t => t.IdProfessor)
                .Index(t => t.IdDisciplina);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Nome = c.String(nullable: false, maxLength: 50),
                        DataNascimento = c.DateTime(nullable: false),
                        NumInterno = c.String(nullable: false),
                        Password = c.String(),
                        Perfil = c.Int(nullable: false),
                        TurmaId = c.Int(),
                        estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Turmas", t => t.TurmaId)
                .Index(t => t.TurmaId);
            
            CreateTable(
                "dbo.Turmas",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        AnoLetivo = c.DateTime(nullable: false),
                        Ano = c.Int(nullable: false),
                        Letra = c.String(nullable: false, maxLength: 2),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notas", "IdProfessor", "dbo.Users");
            DropForeignKey("dbo.Notas", "IdDisciplina", "dbo.Disciplinas");
            DropForeignKey("dbo.Notas", "IdAluno", "dbo.Users");
            DropForeignKey("dbo.Users", "TurmaId", "dbo.Turmas");
            DropIndex("dbo.Users", new[] { "TurmaId" });
            DropIndex("dbo.Notas", new[] { "IdDisciplina" });
            DropIndex("dbo.Notas", new[] { "IdProfessor" });
            DropIndex("dbo.Notas", new[] { "IdAluno" });
            DropTable("dbo.Turmas");
            DropTable("dbo.Users");
            DropTable("dbo.Notas");
            DropTable("dbo.Disciplinas");
        }
    }
}
