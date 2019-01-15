using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.Model.Entity;

namespace TwoFA.Model.Context
{
    public class TwoFaDbContext : DbContext
    {
        public DbSet<Custom_User> CustomUsers { get; set; }
        public DbSet<Manufacturer_User> ManufacturerUsers { get; set; }
        public DbSet<CustomWithManufacturer> CustomWithManufacturers { get; set; }


        //如果Model改变了将删除数据库重新创建数据库
        public TwoFaDbContext():base("name=SqlServerConnectionString")
        {
            Database.SetInitializer<TwoFaDbContext>(new DropCreateDatabaseIfModelChanges<TwoFaDbContext>());
        }

        //创建模型
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Custom_User.MapToDbContext(modelBuilder);

            Manufacturer_User.MapToDbContext(modelBuilder);

            CustomWithManufacturer.MapToDbContext(modelBuilder);
        }
    }
}
