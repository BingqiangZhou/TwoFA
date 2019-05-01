using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.WebMVC.Models.Model;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.Security.Claims;

namespace TwoFA.WebMVC.Models.Context
{
    public class DbInit : DropCreateDatabaseIfModelChanges<TwoFADbContext>
    {
        protected override void Seed(TwoFADbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public static void PerformInitialSetup(TwoFADbContext context)
        {
            // 初始化配置将放在这儿 
            
        }
    }

    public class TwoFADbContext : IdentityDbContext<User>
    {
        public TwoFADbContext() : base("TwoFADb") { }

        static TwoFADbContext()
        {
            Database.SetInitializer(new DbInit());
        }

        public static TwoFADbContext Create()
        {
            return new TwoFADbContext();
        }
    }
}
