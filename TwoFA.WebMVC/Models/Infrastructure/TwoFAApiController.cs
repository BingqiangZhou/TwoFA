using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TwoFA.WebMVC.Models.Context;

namespace TwoFA.WebMVC.Models.Infrastructure
{
    public class TwoFAApiController : ApiController
    {
        //获取用户管理上下文
        public static TwoFAUserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<TwoFAUserManager>();
            }
        }
        //获取角色管理上下文
        public static TwoFARoleManager RoleManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<TwoFARoleManager>();
            }
        }
        public static IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
    }
}
