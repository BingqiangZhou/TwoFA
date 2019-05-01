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
using TwoFA.Utils.ToolsClass;
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
            ViewBag.Name = null;
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
                    var accessToken= GenerateCode.GenerateSHA1(GetTokenById(user.Id));
                    //获取厂商ID和Token
                    string id = ConfigurationManager.AppSettings["Id"];
                    string signatureKey = ConfigurationManager.AppSettings["Token"];
                    string hostURL = ConfigurationManager.AppSettings["hostURL"];
                    string timestamp = Singature.GetTimeStamp();
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("user", user.Name);
                    dict.Add("mId", id);
                    dict.Add("signatureKey", signatureKey);
                    dict.Add("timestamp", timestamp);
                    dict.Add("accessToken", HttpUtility.UrlEncode(accessToken));
                    string sign = Singature.GetSignature(dict);
                    dict.Remove("signatureKey");
                    dict.Add("sign", sign);
                    string urlParamas = Singature.GetUrl(dict);

                    TempData["accessToken"] = accessToken;
                    TempData["user"] = user;

                    return Redirect(hostURL + "/TwoFAValidationService?" + urlParamas);
                    //重定向到验证服务
                }
            }
            //设置错误提示
            ModelState.AddModelError("Email", "邮箱号或密码错误");
            return View("Login", loginModel);
        }
        public ActionResult LoginSuccess(string accessToken)
        {
            //比对accessToken是否匹配
            if (TempData["accessToken"] == null)
            {
                return Content("非法访问！   （001）");
            }
            if (TempData["accessToken"].Equals(accessToken) == false)
            {
                //设置错误提示
                return Content("警告：恶意访问将付法律责任");
            }
            //获取User对象
            User user = (User)TempData["user"];
            if (user == null)
            {
                return Content("非法访问！   （002）");
            }
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