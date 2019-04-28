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
    [Authorize]
    public class ConfigController : TwoFAMVCController
    {
        
        public ActionResult Index()
        {
            ViewBag.Id = ConfigurationManager.AppSettings["Id"];
            ViewBag.Token = ConfigurationManager.AppSettings["Token"];
            User user = HaveUserLogined();
            if (user == null)
            {
                return Content("你还没有登录");
            }
            string url = GetReturnURLById(user.Id);
            return View("Index",new ConfigModel {userName =user.UserName,mId=user.Id,
                serviceIsOpen =(user.OpenID!=null&&user.OpenID.Length != 0?true:false),mUrl = url });
        }


        #region GetToken，GetResetKey，SetReturnURL改为WebAPI
        [HttpPost]
        public ActionResult GetToken()
        {
            string result = "Error";
            User user = HaveUserLogined();
            if (user != null)
            {
                result = user.SecurityStamp;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult GetResetKey()
        {
            string result = "Error";
            User user = HaveUserLogined();
            if (user != null)
            {
                result = user.ResetKey;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult SetReturnURL(string url)
        {
            string result = "Error";
            User user = HaveUserLogined();
            if (user != null)
            {
                bool setResult = SetReturnURLById(user.Id, url);
                if (true == setResult)
                {
                    result = true.ToString();
                }
            }
            return Json(result);
        }
        #endregion

    }
}