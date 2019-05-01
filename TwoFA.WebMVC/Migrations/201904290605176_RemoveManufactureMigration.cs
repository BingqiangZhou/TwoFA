namespace TwoFA.WebMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveManufactureMigration : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Manufacture");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Manufacture", c => c.String());
        }
    }
}
