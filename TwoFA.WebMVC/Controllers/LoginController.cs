using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
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
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(loginModel.Email);
                if (user != null)
                {
                    var u = UserManager.Find(user.UserName, loginModel.Password);
                    if (u != null)
                    {   
                        var accessToken= await UserManager.GenerateEmailConfirmationTokenAsync(u.Id);
                        TempData["accessToken"] = accessToken;
                        TempData["user"] = u;
                        return RedirectToAction("Index", "TwoFAValidationService",
                            new VerifyModel { userName=u.UserName ,mId="TwoFA",token="Default",accessToken= accessToken });
                    }
                }
                else {
                    ModelState.AddModelError("Email", "邮箱号或密码错误");
                }
            }
            else
            {
                ModelState.AddModelError("Email", "邮箱号或密码错误");
            }
            return View("Login", loginModel);
        }
        public async Task<ActionResult> LoginSuccess(string accessToken)
        {
            if (TempData["accessToken"].Equals(accessToken) == false)
            {
                return Content("警告：恶意访问将付法律责任");
            }
            User u = (User)TempData["user"];
            ClaimsIdentity ident = await UserManager.CreateIdentityAsync(u, DefaultAuthenticationTypes.ApplicationCookie);
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
            //HttpContext.Response.Cookies.Add(new HttpCookie("UserName",u.UserName));
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //var cookie = Request.Cookies["UserName"];
            //cookie.Expires = DateTime.Now.AddDays(-1);
            //Response.Cookies.Add(cookie);
            return RedirectToAction("Index", "Home");
        }
    }
}