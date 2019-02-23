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

namespace TwoFA.WebMVC.Models.Context
{
    public class DbInit : DropCreateDatabaseIfModelChanges<IdentityDbContext>
    {
        protected override void Seed(IdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public static void PerformInitialSetup(IdentityDbContext context)
        {
            // 初始化配置将放在这儿 
            //获取用户管理上下文
            var userMgr = HttpContext.Current.GetOwinContext().GetUserManager<TwoFAUserManager>();
            //获取角色管理上下文
            var roleMgr = HttpContext.Current.GetOwinContext().Get<TwoFARoleManager>();

            //厂商角色名
            const string manufactruerRoleName = "M";
            //普通用户角色名
            const string ordinaryUserRoleName = "O";
            //当前APP名（用于厂商的初始使用）
            const string appUserName = "TwoFA";

            //厂商用户角色不存在，创建角色
            var manufactruerRole = roleMgr.FindByName(manufactruerRoleName);
            if (manufactruerRole == null)
            {
                roleMgr.Create(new Role(manufactruerRoleName));
            }
            //厂商用户的用户角色不存在，创建角色
            var ordinaryUserRole = roleMgr.FindByName(ordinaryUserRoleName);
            if (ordinaryUserRole == null)
            {
                roleMgr.Create(new Role(ordinaryUserRoleName));
            }
            //不存在则创建用户
            var appUser = userMgr.FindByName(appUserName);
            if (appUser == null)
            {
                userMgr.Create(new User { UserName = appUserName });
                appUser = userMgr.FindByName(appUserName);
            }
            //将应用信息添加到AppSetting
            ConfigurationManager.AppSettings["Id"] = appUser.Id;
            ConfigurationManager.AppSettings["Token"] = appUser.SecurityStamp;
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
