using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFA.Model.Entity
{
    /// <summary>
    /// 小程序用户信息
    /// </summary>
    public class Custom_User:User
    {
        public static void MapToDbContext(DbModelBuilder modelBuilder)
        {
            //TPC(Table per Concerete class)
            var table = modelBuilder.Entity<Custom_User>().Map(m=>{
                m.MapInheritedProperties();
            });
            //约束
            table.ToTable("CustomUser");
            //customUser.Property(c=>c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            table.HasKey(c => c.Id);//默认自增
            table.Property(c => c.Name).HasMaxLength(16);
        }
    }
}
