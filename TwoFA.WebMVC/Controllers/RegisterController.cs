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
            User user = HaveUserLogined();
            ViewBag.Name = null;
            if (user != null)
            {
                ViewBag.Name = user.Name;
            }
            return View("register1");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Step2(RegisterModel registerModel)
        {
            //查询企业是否已经注册
            string id = FindUserIdByName(registerModel.Name);
            User user = FindUserById(id);
            if (user != null)
            {
                ModelState.AddModelError("Name", "该企业已注册");
                return View("register1", registerModel);
            }
            //查询邮箱是否已经被注册
            user = FindUserByEmail(registerModel.Email);
            if (user != null)
            {
                ModelState.AddModelError("Email", "该邮箱已注册");
                return View("register1", registerModel);
            }
            //生成6位验证码
            registerModel.Code = GenerateCode.GenerateEmailCode(100000, 999999);
            //发送到邮件
            bool res = SendCodeToEmail.SendCode(registerModel.Email, registerModel.Code.ToString());
            if (res == true)
            {
                //进入第二步
                TempData["Model"] = registerModel;
                return View("register2", new RegisterModel { Email=registerModel.Email });
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
            RegisterModel info =(RegisterModel)TempData["Model"];
            if (info.Code != registerModel.Code)
            {
                ModelState.AddModelError("Code", "请输入正确的验证码！");
            }
            //获取Id（厂商Id）
            string appId = ConfigurationManager.AppSettings["Id"];
            //创建用户
            bool result = CreateUser(info.Email, info.Name, info.Password,appId);
            if (result == true)
            {
                User user = FindUserByEmail(info.Email);
                //设置角色
                AddRoleToManufactruerById(user.Id);
                ViewBag.Id = ConfigurationManager.AppSettings["Id"];
                ViewBag.Token = ConfigurationManager.AppSettings["Token"];
                return View("register3", info);
            }
            else
            {
                ModelState.AddModelError("Code", "出现未知错误,请与技术人员联系");
                return View("register2", registerModel);
            }
        }
    }
}