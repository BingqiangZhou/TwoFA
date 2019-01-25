using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.WebMVC.Models.Context;
using TwoFA.WebMVC.Models.Model;

namespace TwoFA.WebMVC.Models.Context
{
    public class TwoFARoleManager : RoleManager<Role>, IDisposable
    {

        public TwoFARoleManager(RoleStore<Role> store) : base(store) { }

        public static TwoFARoleManager Create(IdentityFactoryOptions<TwoFARoleManager> options, IOwinContext context)
        {
            return new TwoFARoleManager(new RoleStore<Role>(context.Get<TwoFADbContext>()));
        }
    }
}
