namespace TwoFA.WebMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Key : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Key");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Key", c => c.String());
        }
    }
}
