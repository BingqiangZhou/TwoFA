using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.Models.Model;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class LoginController : TwoFAMVCController
    {
        [HttpGet]
        [AllowAnonymous]
        //登录页面
        public ActionResult Login()
        {
            //判断是否有用户已经登录，如果有用户已经登录，转到首页，不然转到登录页面
            if (null != HaveUserLogined())
            {
                return View("Index","Home");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        //判断登录提交的数据，做出相应的响应
        public ActionResult Login(LoginModel loginModel)
        {
            //验证model数据是否有效
            if (ModelState.IsValid)
            {
                //验证账号
                var user = VerifyAccountByEmailAndPassword(loginModel.Email, loginModel.Password);
                if (user != null)
                {
                    //获取token
                    var accessToken= GetTokenById(user.Id);
                    //将token和user对象存到缓存
                    TempData["accessToken"] = accessToken;
                    TempData["user"] = user;
                    //获取厂商ID和Token
                    string mId = ConfigurationManager.AppSettings["Id"];
                    string token = ConfigurationManager.AppSettings["Token"];
                    //解码用户名
                    string userName = DecodeUserName(user);
                    //重定向到验证服务
                    return RedirectToAction("Index", "TwoFAValidationService",
                        new VerifyModel { userName= userName,mId= mId, token= token, accessToken= accessToken });
                }
            }
            //设置错误提示
            ModelState.AddModelError("Email", "邮箱号或密码错误");
            return View("Login", loginModel);
        }
        public ActionResult LoginSuccess(string accessToken)
        {
            //比对accessToken是否匹配
            if (TempData["accessToken"].Equals(accessToken) == false)
            {
                //设置错误提示
                return Content("警告：恶意访问将付法律责任");
            }
            //获取User对象
            User user = (User)TempData["user"];
            //用户登录
            UserSignIn(user);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            //用户退出登录
            UserSignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}