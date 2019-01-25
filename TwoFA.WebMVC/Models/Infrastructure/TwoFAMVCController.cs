using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Context;

namespace TwoFA.WebMVC.Models.Infrastructure
{
    public class TwoFAMVCController : Controller
    {
        //获取用户管理上下文
        public TwoFAUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<TwoFAUserManager>();
            }
        }
        //获取角色管理上下文
        public TwoFARoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<TwoFARoleManager>();
            }
        }
        public IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}