namespace M17E_Caderneta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NomeCompleto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "NumTurma", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "NumTurma");
        }
    }
}
