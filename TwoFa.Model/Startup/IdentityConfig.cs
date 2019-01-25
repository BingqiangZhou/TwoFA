using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.Model.Context;

namespace TwoFA.Model.Startup
{
    class IdentityConfig
    {
        /// <summary>
        /// 为生成token提供Provider
        /// </summary>
        public static IDataProtectionProvider DataProtectionProvider { get; set; }
        /// <summary>
        /// Owin启动配置
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            //创建数据库上下文
            app.CreatePerOwinContext<TwoFADbContext>(TwoFADbContext.Create);
            //创建用户管理
            app.CreatePerOwinContext<TwoFAUserManager>(TwoFAUserManager.Create);
            //创建角色管理
            app.CreatePerOwinContext<TwoFARoleManager>(TwoFARoleManager.Create);

            //使用cookie验证方式，并默认登录页面为/Account/Login
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
            DataProtectionProvider = app.GetDataProtectionProvider();
        }
    }
}
