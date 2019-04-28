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
using TwoFA.WebMVC.Models.Model;
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
        public ActionResult Forget(ForgetPasswordModel forgetPassowrdModel)
        {
            User user = FindUserByEmail(forgetPassowrdModel.Email);
            //验证邮箱与用户名
            if (user == null || false == DecodeUserName(user).Equals(forgetPassowrdModel.Name))
            {
                //设置错误提示
                ModelState.AddModelError("Email", "信息不匹配，无法进行下一步操作");
                return View("Forget1",forgetPassowrdModel);
            }
            //获取重置密码token
            string token = GeneratePasswordResetTokenById(user.Id);
            if(token == null)
            {
                //设置错误提示
                ModelState.AddModelError("Email", "内部程序错误，请重试，如多次尝试无效，请联系技术人员");
                return View("Forget1", forgetPassowrdModel);
            }
            //构造url，并发送邮件
            string url = ConfigurationManager.AppSettings["SiteURL"];
            token = HttpUtility.UrlEncode(token);
            url = url + "Password/ForgetPassword?name=" + user.UserName + "&token=" + token;
            bool res = SendCodeToEmail.ModifyPassword(forgetPassowrdModel.Email,url);
            if (res == false)
            {
                //设置错误提示
                ModelState.AddModelError("Email", "验证邮件发送出现异常,请联系相关技术人员");
                return View("Forget1", forgetPassowrdModel);
            }
            //设置重置密码token
            bool result = SetPasswordResetTokenById(user.Id, token);
            if (result== false)
            {
                ModelState.AddModelError("Email", "出现未知异常,请联系相关技术人员");
                return View("Forget1", forgetPassowrdModel);
            }
            return View("Forget2");
        }
        [HttpGet]
        public ActionResult ForgetPassword(string name,string token)
        {
            //查找用户
            User user = FindUserByUserName(name);
            if (user == null)
            {
                ModelState.AddModelError("Email", "链接不正确，请勿修改链接");
                return View("Forget1");
            }
            //验证token是否匹配
            bool result = VerifyPasswordResetToken(user.Id, token);
            if (result != true)
            { 
                return Content("该链接已失效");
            }
            return View("Forget3",new ForgetPasswordConfirmModel { Id = user.Id,Token=token});
        }
        public ActionResult ForgetAndConfirmPassword(ForgetPasswordConfirmModel forgetPasswordConfirmModel)
        {
            //string token = HttpUtility.UrlEncode(forgetPasswordConfirmModel.Token);
            bool result = ResetPassword(forgetPasswordConfirmModel.Id,
                forgetPasswordConfirmModel.Token, forgetPasswordConfirmModel.Password);
            if (result == false)
            {
                ModelState.AddModelError("Password", "密码保存失败");
                return View("Forget3", forgetPasswordConfirmModel);
            }
            return View("Forget4");
        }
        [HttpGet]
        [Authorize]
        public ActionResult Modify()
        {
            return View("Modify1",new ModifyPasswordModel { Name=HttpContext.User.Identity.Name });
        }
        [HttpPost]
        [Authorize]
        public ActionResult Modify(ModifyPasswordModel modifyPasswordModel)
        {
            //查找用户
            User user = FindUserByUserName(modifyPasswordModel.Name);
            if (user == null)
            {
                ModelState.AddModelError("ConfirmPassword", "出现未知错误,请联系相关技术人员");
                return View("Modify1", modifyPasswordModel);
            }
            //用户修改密码
            var result = ChangePassword(user.Id,modifyPasswordModel.OldPassword, modifyPasswordModel.NewPassword);
            if (result == false)
            {
                ModelState.AddModelError("ConfirmPassword", "请输入正确的密码");
                return View("Modify1", modifyPasswordModel);
            }
            //用户退出登录
            UserSignOut();
            return View("Modify2", modifyPasswordModel);
        }
    }
}