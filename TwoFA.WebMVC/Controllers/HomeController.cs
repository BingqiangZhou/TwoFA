using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Context;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.Models.Model;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class HomeController : TwoFAMVCController
    {
        public ActionResult Index()
        {
            ViewBag.Name = null;
            User user = HaveUserLogined();
            if (user != null)
            {
                ViewBag.Name = user.Name;
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Detail()
        {
            return View("twofa_detail");
        }

        public ActionResult Init()
        {
            try
            {
                //获取用户管理上下文
                var userMgr = HttpContext.GetOwinContext().GetUserManager<TwoFAUserManager>();
                //获取角色管理上下文
                var roleMgr = HttpContext.GetOwinContext().Get<TwoFARoleManager>();

                //厂商角色名
                const string manufactruerRoleName = "Manufactruer";
                //普通用户角色名
                const string ordinaryUserRoleName = "OrdinaryUser";
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
                    userMgr.Create(new User { UserName = appUserName,
                        Id = ConfigurationManager.AppSettings["Id"],
                        SecurityStamp = ConfigurationManager.AppSettings["Token"],Name=appUserName
                    });
                    appUser = userMgr.FindByName(appUserName);
                }
                userMgr.AddClaim(appUser.Id, new Claim("ReturnUrl",ConfigurationManager.AppSettings["HostURL    "] + "/Login/LoginSuccess"));
                return Content("初始化成功");
            }
            catch
            {
                return Content("初始化失败");
            }
        }
    }
}