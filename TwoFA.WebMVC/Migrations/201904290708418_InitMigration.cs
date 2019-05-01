namespace TwoFA.WebMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetCustomUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        OpenId = c.String(),
                        ResetKey = c.String(),
                        Manufacture = c.String(),
                        TwoFAKey = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.AspNetUsers", "Test");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Test", c => c.String());
            DropTable("dbo.AspNetCustomUsers");
        }
    }
}
