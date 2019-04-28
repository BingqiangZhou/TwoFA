namespace Exhibition.Migrations
{
    using Exhibition.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Exhibition.Context.ExhibitionDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Exhibition.Context.ExhibitionDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.users.AddOrUpdate(new User() { Name = "test", Password = "123456" });
            context.SaveChanges();
        }
    }
}
