namespace M17E_Caderneta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alteracoes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Disciplinas", "Descricao", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Disciplinas", "Nome", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Disciplinas", "Nome", c => c.String(nullable: false));
            DropColumn("dbo.Disciplinas", "Descricao");
        }
    }
}
