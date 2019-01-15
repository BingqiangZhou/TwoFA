using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFA.Model.Entity
{
    public class CustomWithManufacturer
    {
        public int CustomWithManufacturerId { get; set; }
        public int CustomeUserId { get; set; }
        public int ManufacturerUserId { get; set; }

        /// <summary>
        /// 用户在厂商的Id
        /// </summary>
        public string IdFromManufacturer { get; set; }
        public string Key { get; set; }

        public static void MapToDbContext(DbModelBuilder modelBuilder)
        {
            var table = modelBuilder.Entity<CustomWithManufacturer>();
            //约束
            table.ToTable("CustomWithManufacturer");
            //customUser.Property(c=>c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            table.HasKey(c => c.CustomWithManufacturerId);//默认自增
        }
    }
}
