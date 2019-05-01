namespace TwoFA.WebMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _001Migration : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.AspNetCustomUsers");
        }
        
        public override void Down()
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
            
        }
    }
}
