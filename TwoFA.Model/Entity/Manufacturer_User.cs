using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFA.Model.Entity
{
    /// <summary>
    /// 厂商实体类
    /// </summary>
    public class Manufacturer_User:User
    {
        /// <summary>
        /// 厂商登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 厂商使用API的Token（采用Token+Id验证）
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 两步验证key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 厂商是否使用两步验证
        /// </summary>
        public bool UseTwoFa { get; set; }

        public static void MapToDbContext(DbModelBuilder modelBuilder)
        {
            //TPC(Table per Concerete class)
            var table = modelBuilder.Entity<Manufacturer_User>().Map(m => {
                m.MapInheritedProperties();
            });
            //约束
            table.ToTable("ManufacturerUser");
            //customUser.Property(c=>c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            table.HasKey(c => c.Id);//默认自增
            table.Property(c => c.Name).HasMaxLength(16);
        }
    }
}
