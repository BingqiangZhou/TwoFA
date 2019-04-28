using Exhibition.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Exhibition.Context
{
    public class ExhibitionDbContext:DbContext
    {
        public ExhibitionDbContext():base("ExhibitionDb")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ExhibitionDbContext>());
        }

        public DbSet<User> users { get; set; }
    }
}