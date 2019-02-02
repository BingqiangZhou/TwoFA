namespace TwoFA.WebMVC.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TwoFA.WebMVC.Models.Model;

    internal sealed class Configuration : DbMigrationsConfiguration<TwoFA.WebMVC.Models.Context.TwoFADbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TwoFA.WebMVC.Models.Context.TwoFADbContext";
        }

        protected override void Seed(TwoFA.WebMVC.Models.Context.TwoFADbContext context)
        {
            //  This method will be called after migrating to the latest version.

            context.Roles.AddOrUpdate(new Role { Name = "M" }, new Role { Name = "U" });

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}