namespace M17E_Caderneta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NumTurmaNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "NumTurma", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "NumTurma", c => c.Int(nullable: false));
        }
    }
}
