namespace M17E_Caderneta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lnkRecuperar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "lnkRecuperar", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "lnkRecuperar");
        }
    }
}
