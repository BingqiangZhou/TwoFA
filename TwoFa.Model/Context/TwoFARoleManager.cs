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

namespace TwoFA.Model.Context
{
    class TwoFARoleManager : RoleManager<Role>, IDisposable
    {

        public TwoFARoleManager(RoleStore<Role> store) : base(store) { }

        public static TwoFARoleManager Create(IdentityFactoryOptions<TwoFARoleManager> options, IOwinContext context)
        {
            return new TwoFARoleManager(new RoleStore<Role>(context.Get<TwoFADbContext>()));
        }
    }
}
