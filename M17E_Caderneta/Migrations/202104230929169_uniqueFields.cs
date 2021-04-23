namespace M17E_Caderneta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uniqueFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "NumInterno", c => c.String(nullable: false, maxLength: 10));
            CreateIndex("dbo.Users", "Email", unique: true, name: "Ix_emailUser");
            CreateIndex("dbo.Users", "NumInterno", unique: true, name: "Ix_NumInternoUser");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "Ix_NumInternoUser");
            DropIndex("dbo.Users", "Ix_emailUser");
            AlterColumn("dbo.Users", "NumInterno", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
        }
    }
}
