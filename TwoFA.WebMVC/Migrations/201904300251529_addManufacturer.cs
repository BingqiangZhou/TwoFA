namespace TwoFA.WebMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addManufacturer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Manufacturer", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Manufacturer");
        }
    }
}
