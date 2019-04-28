using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.ViewModel;
using TwoFA.Utils.ToolsClass;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using TwoFA.WebMVC.Models.Model;
using System.Configuration;

namespace TwoFA.WebMVC.Controllers
{
    public class RegisterController : TwoFAMVCController
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Step1()
        {
            return View("register1");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Step2(RegisterModel registerModel)
        {
            var user = FindUserByUserName(registerModel.Name);
            if (user != null)
            {
                ModelState.AddModelError("Name", "该企业已注册");
                return View("register1", registerModel);
            }
            user = FindUserByEmail(registerModel.Email);
            if (user != null)
            {
                ModelState.AddModelError("Email", "该邮箱已注册");
                return View("register1", registerModel);
            }
            registerModel.Code = GenerateCode.GenerateEmailCode(100000, 999999);
            bool res = SendCodeToEmail.SendCode(registerModel.Email, registerModel.Code.ToString());
            if (res == true)
            {
                return View("register2", registerModel);
            }
            else
            {
                ModelState.AddModelError("Email", "验证邮件发送出现异常,请检查您的邮箱!");
                return View("register1", registerModel);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Step3(RegisterModel registerModel)
        {
            //获取Id（厂商Id）
            string appId = ConfigurationManager.AppSettings["Id"];
            //用户名
            string userName = EncodeUserName(appId,registerModel.Name);
            //创建用户
            bool result = CreateUser(registerModel.Email,userName,registerModel.Password);
            if (result == true)
            {
                User user = FindUserByEmail(registerModel.Email);
                //设置角色
                AddRoleToManufactruerById(user.Id);
                ViewBag.Id = ConfigurationManager.AppSettings["Id"];
                ViewBag.Token = ConfigurationManager.AppSettings["Token"];
                return View("register3",registerModel);
            }
            else
            {
                ModelState.AddModelError("VeifyCode", "出现未知错误,请与技术人员联系");
                return View("register2", registerModel);
            }
        }
    }
}