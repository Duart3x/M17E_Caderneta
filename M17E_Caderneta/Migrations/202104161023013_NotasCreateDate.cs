namespace M17E_Caderneta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotasCreateDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notas", "CreateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notas", "CreateDate");
        }
    }
}
