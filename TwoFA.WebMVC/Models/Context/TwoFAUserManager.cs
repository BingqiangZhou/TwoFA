using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.WebMVC.Models.Context;
using TwoFA.WebMVC.Models.Model;
using TwoFA.WebMVC.OwinStartup;

namespace TwoFA.WebMVC.Models.Context
{
    public class TwoFAUserManager : UserManager<User>
    {
        public TwoFAUserManager(IUserStore<User> store) : base(store) { }

        public static TwoFAUserManager Create(IdentityFactoryOptions<TwoFAUserManager> options
            , IOwinContext context)
        {
            TwoFADbContext db = context.Get<TwoFADbContext>();
            TwoFAUserManager manager = new TwoFAUserManager(new UserStore<User>(db));

            var dataProtectorProvider = new DpapiDataProtectionProvider("TwoFA");
            //IdentityConfig.DataProtectionProvider;
            var dataProtector = dataProtectorProvider.Create("BingqiangZhou");
            manager.UserTokenProvider = new DataProtectorTokenProvider<User, string>(dataProtector);
            return manager;
        }
    }
}
