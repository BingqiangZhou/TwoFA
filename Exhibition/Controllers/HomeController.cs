using Exhibition.Context;
using Exhibition.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.Utils.ToolsClass;

namespace Exhibition.Controllers
{
    public class HomeController : Controller
    {
        private ExhibitionDbContext db = new ExhibitionDbContext();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowQRCode()
        {
            return View();
        }
        public ActionResult CodeCompare()
        {
            return View();
        }

        public ActionResult SignatureWay()
        {
            return View();
        }

        public ActionResult OpenTwoFAService()
        {
            string host = ConfigurationManager.AppSettings["TwoFAHost"];
            string id = ConfigurationManager.AppSettings["Id"];
            string token = ConfigurationManager.AppSettings["Token"];
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", "1");                      //传入用户唯一标识，这里省略查询用户数据步骤，传入用户id
            dict.Add("mId", id);                        //这里传企业两步验证账号id
            dict.Add("signatureKey", token);            //这里传入企业两步验证token
            string timestamp = Singature.GetTimeStamp();//获取当前时间戳
            dict.Add("timestamp", timestamp);           //传入当前时间戳，当前时间到1970-1-1的秒数
            string sign = Singature.GetSignature(dict); //获取到sign
            dict.Remove("signatureKey");
            dict.Add("sign", sign);
            string urlParam = Singature.GetUrl(dict);   //构造参数链接
            return Redirect(host + "OpenTwoFAService?" + urlParam);
        }
        public ActionResult CloseTwoFAService()
        {
            string host = ConfigurationManager.AppSettings["TwoFAHost"];
            string id = ConfigurationManager.AppSettings["Id"];
            string token = ConfigurationManager.AppSettings["Token"];
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", "1");                      //传入用户唯一标识，这里省略查询用户数据步骤，传入用户id
            dict.Add("mId", id);                        //这里传企业两步验证账号id
            dict.Add("signatureKey", token);            //这里传入企业两步验证token
            string timestamp = Singature.GetTimeStamp();//获取当前时间戳
            dict.Add("timestamp", timestamp);           //传入当前时间戳，当前时间到1970-1-1的秒数
            string sign = Singature.GetSignature(dict); //获取到sign
            dict.Remove("signatureKey");
            dict.Add("sign", sign);
            string urlParam = Singature.GetUrl(dict);   //构造参数链接
            return Redirect(host + "TwoFAResetService?" + urlParam);
        }
    }
}