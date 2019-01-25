using Microsoft.AspNet.Identity;
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
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class PasswordController : TwoFAMVCController
    {

        [HttpGet]
        public ActionResult Forget()
        {
            return View("Forget1");
        }
        [HttpPost]        
        public async Task<ActionResult> Forget(ForgetPasswordModel forgetPassowrdModel)
        {
            var user = await UserManager.FindByEmailAsync(forgetPassowrdModel.Email);
            if (user == null || false == user.UserName.Equals(forgetPassowrdModel.Name))
            {
                ModelState.AddModelError("Email", "信息不匹配，无法进行下一步操作");
                return View("Forget1",forgetPassowrdModel);
            }
            string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            string url = ConfigurationManager.AppSettings["SiteURL"];
            token = HttpUtility.UrlEncode(token);
            url = url + "Password/ForgetPassword?name=" + forgetPassowrdModel.Name + "&token=" + token;
            bool res = SendCodeToEmail.ModifyPassword(forgetPassowrdModel.Email,url);
            if (res == false)
            {
                ModelState.AddModelError("Email", "验证邮件发送出现异常,请联系相关技术人员");
                return View("Forget1", forgetPassowrdModel);
            }
            var result = await UserManager.AddClaimAsync(user.Id, new Claim("PasswordResetToken", token));
            if (result.Succeeded == false)
            {
                ModelState.AddModelError("Email", "出现未知异常,请联系相关技术人员");
                return View("Forget1", forgetPassowrdModel);
            }
            return View("Forget2");
        }
        [HttpGet]
        public async  Task<ActionResult> ForgetPassword(string name,string token)
        {
            var user = await UserManager.FindByNameAsync(name);
            if (user == null)
            {
                ModelState.AddModelError("Email", "链接不正确，请勿修改链接");
                return View("Forget1");
            }
            var claims = await UserManager.GetClaimsAsync(user.Id);
            string tokenTmp = "0";
            Claim claimTmp = null;
            foreach (var claim in claims)
            {
                if (claim.Type == "PasswordResetToken")
                {
                    tokenTmp = HttpUtility.UrlDecode(claim.Value);
                    claimTmp = claim;
                }
            }
            //token = HttpUtility.UrlDecode(token);
            if (tokenTmp.Equals(token) == true)
            {
                var res = await UserManager.RemoveClaimAsync(user.Id, claimTmp);
                if (res.Succeeded == false)
                {
                    return Content("该链接已失效01");
                }
            }
            else
            {
                return Content("该链接已失效02");
            }
            return View("Forget3",new ForgetPasswordConfirmModel { Id = user.Id,Token=token});
        }
        public async Task<ActionResult> ForgetAndConfirmPassword(ForgetPasswordConfirmModel forgetPasswordConfirmModel)
        {
            //string token = HttpUtility.UrlEncode(forgetPasswordConfirmModel.Token);
            IdentityResult result = await UserManager.ResetPasswordAsync(forgetPasswordConfirmModel.Id,
                forgetPasswordConfirmModel.Token, forgetPasswordConfirmModel.Password);
            if (result.Succeeded == false)
            {
                ModelState.AddModelError("Password", "密码保存失败");
                return View("Forget3", forgetPasswordConfirmModel);
            }
            return View("Forget4");
        }
        [HttpGet]
        [Authorize]
        public ActionResult Modify(string name)
        {
            return View("Modify1",new ModifyPasswordModel { Name = name});
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Modify(ModifyPasswordModel modifyPasswordModel)
        {
            var user = await UserManager.FindByNameAsync(modifyPasswordModel.Name);
            if (user == null)
            {
                ModelState.AddModelError("ConfirmPassword", "出现未知错误,请联系相关技术人员");
                return View("Modify1", modifyPasswordModel);
            }
            var res = await UserManager.ChangePasswordAsync(user.Id,
                modifyPasswordModel.OldPassword, modifyPasswordModel.NewPassword);
            if (res.Succeeded == false)
            {
                ModelState.AddModelError("ConfirmPassword", "请输入正确的密码");
                return View("Modify1", modifyPasswordModel);
            }
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var cookie = Request.Cookies["UserName"];
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            return View("Modify2", modifyPasswordModel);
        }
    }
}