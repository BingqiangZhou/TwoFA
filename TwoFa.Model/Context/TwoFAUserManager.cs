using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.Model.Model;
using TwoFA.Model.Startup;

namespace TwoFA.Model.Context
{
    class TwoFAUserManager : UserManager<User>
    {
        public TwoFAUserManager(IUserStore<User> store) : base(store) { }

        public static TwoFAUserManager Create(IdentityFactoryOptions<TwoFAUserManager> options
            , IOwinContext context)
        {
            TwoFADbContext db = context.Get<TwoFADbContext>();
            TwoFAUserManager manager = new TwoFAUserManager(new UserStore<User>(db));

            var dataProtectorProvider = IdentityConfig.DataProtectionProvider;
            var dataProtector = dataProtectorProvider.Create("BingqiangZhou");
            manager.UserTokenProvider = new DataProtectorTokenProvider<User, string>(dataProtector)
            {
                TokenLifespan = TimeSpan.MaxValue,
            };

            return manager;
        }
    }
}
