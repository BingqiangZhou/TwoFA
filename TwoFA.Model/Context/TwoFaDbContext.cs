using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.Model.Model;

namespace TwoFA.Model.Context
{
    public class DbInit : DropCreateDatabaseIfModelChanges<IdentityDbContext>
    {
        protected override void Seed(IdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public void PerformInitialSetup(IdentityDbContext context)
        {
            // 初始化配置将放在这儿 
        }
    }

    class TwoFADbContext : IdentityDbContext<User>
    {
        public TwoFADbContext() : base("TwoDb") { }

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
