using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.WebMVC.Models.Model;
using Microsoft.AspNet.Identity;

namespace TwoFA.WebMVC.Models.Context
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
            //TwoFAUserManager userMgr = new TwoFAUserManager(new UserStore<User>(context));
            TwoFARoleManager roleMgr = new TwoFARoleManager(new RoleStore<Role>(context));

            //userMgr.CreateAsync(new User { UserName = "miniProgram" });
            //userMgr.CreateAsync(new User { UserName = "manufacturuer" });

            roleMgr.Create(new Role("M"));
            roleMgr.Create(new Role("U"));

            //userMgr.AddToRoleAsync(userMgr.FindByNameAsync("miniProgram").Result.Id, "CustomerUser");
            //userMgr.AddToRoleAsync(userMgr.FindByNameAsync("manufacturuer").Result.Id, "Manufacturuer");

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
