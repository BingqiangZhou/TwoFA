using Microsoft.Owin.Security;
using System.Web;
using TwoFA.Model.Context;

namespace TwoFA.Model.Infrastructure
{
    class TwoFAContext
    {
        //获取用户管理上下文
        public static TwoFAUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<TwoFAUserManager>();
            }
        }
        //获取角色管理上下文
        public static TwoFARoleManager RoleManager
        {
            get
            {
                return HttpContent.GetOwinContext().GetUserManager<TwoFARoleManager>();
            }
        }
        public static IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}
