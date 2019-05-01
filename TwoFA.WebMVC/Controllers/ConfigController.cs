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
    [Authorize]
    public class ConfigController : TwoFAMVCController
    {
        
        public ActionResult Index()
        {
            ViewBag.Id = ConfigurationManager.AppSettings["Id"];
            ViewBag.Token = ConfigurationManager.AppSettings["Token"];
            User user = HaveUserLogined();
            string url = GetReturnURLById(user.Id);
            ViewBag.Name = user.Name;
            return View("Index",new ConfigModel {userName =user.UserName,mId=user.Id,
                serviceIsOpen =(user.OpenId!=null&&user.OpenId.Length != 0?true:false),mUrl = url });
        }

        public ActionResult OpenTwoFAService()
        {
            User user = HaveUserLogined();
            string id = ConfigurationManager.AppSettings["Id"];
            string signatureKey = ConfigurationManager.AppSettings["Token"];
            string hostURL = ConfigurationManager.AppSettings["HostURL"];
            string timestamp = Singature.GetTimeStamp();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", user.Name);
            dict.Add("mId", id);
            dict.Add("signatureKey", signatureKey);
            dict.Add("timestamp", timestamp);
            string sign = Singature.GetSignature(dict);
            dict.Remove("signatureKey");
            dict.Add("sign", sign);
            string urlParamas = Singature.GetUrl(dict);
            return Redirect(hostURL + "/OpenTwoFAService?" + urlParamas);
        }

        public ActionResult CloseTwoFAService()
        {
            User user = HaveUserLogined();
            string id = ConfigurationManager.AppSettings["Id"];
            string signatureKey = ConfigurationManager.AppSettings["Token"];
            string hostURL = ConfigurationManager.AppSettings["HostURL"];
            string timestamp = Singature.GetTimeStamp();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", user.Name);
            dict.Add("mId", id);
            dict.Add("signatureKey", signatureKey);
            dict.Add("timestamp", timestamp);
            string sign = Singature.GetSignature(dict);
            dict.Remove("signatureKey");
            dict.Add("sign", sign);
            string urlParamas = Singature.GetUrl(dict);
            return Redirect(hostURL + "/TwoFAResetService?" + urlParamas);
        }

        #region 
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