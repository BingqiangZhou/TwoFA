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
    public class HaveTwoFAController : Controller
    {
        private ExhibitionDbContext db = new ExhibitionDbContext();
        // GET: HaveTwoFA
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            var u = db.users.AsNoTracking().Where(m => m.Name == user.Name && m.Password == user.Password)
                    .FirstOrDefault();
            if (u != null)
            {
                string host = ConfigurationManager.AppSettings["TwoFAHost"];
                string id = ConfigurationManager.AppSettings["Id"];
                string token = ConfigurationManager.AppSettings["Token"];
                string accessToken = "888888";              //这里的“888888”，只是标识
                                                            //实际应该包括用户信息并加密，最后在回置控制器中提取用户信息
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("user", u.Id.ToString());          //传入用户唯一标识，这里省略查询用户数据步骤，传入用户id
                dict.Add("mId", id);                        //这里传企业两步验证账号id
                dict.Add("signatureKey", token);            //这里传入企业两步验证token
                dict.Add("accessToken", accessToken);       //这里传入回置链接参数accessToken
                string timestamp = Singature.GetTimeStamp();//获取当前时间戳
                dict.Add("timestamp", timestamp);           //传入当前时间戳，当前时间到1970-1-1的秒数
                string sign = Singature.GetSignature(dict); //获取到sign
                dict.Remove("signatureKey");
                dict.Add("sign", sign);
                string urlParam = Singature.GetUrl(dict);   //构造参数链接
                return Redirect(host + "TwoFAValidationService?" + urlParam);
            }
            ModelState.AddModelError("Password", "账号密码不正确！");
            return View("Index");
        }
        public ActionResult LoginSuccess(string assessToken)
        {
            if (assessToken == "888888")
            {
                return Content("accessToken不对");
            }
            return View();
        }
    }
}