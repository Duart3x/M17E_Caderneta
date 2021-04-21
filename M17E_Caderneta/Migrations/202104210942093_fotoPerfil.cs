namespace M17E_Caderneta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fotoPerfil : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "foto", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "foto");
        }
    }
}
