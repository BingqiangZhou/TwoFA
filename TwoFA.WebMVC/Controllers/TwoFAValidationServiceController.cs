using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwoFA.Utils.ToolsClass;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.Models.Model;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class TwoFAValidationServiceController : TwoFAMVCController
    {
        // GET: TwoFAValidationService
        public ActionResult Index(SignatureModel model,string accessToken)
        {
            string timestamp = Singature.GetTimeStamp();
            int timestampParams = int.Parse(model.timestamp);
            //超出十分钟无效
            if (timestampParams - int.Parse(timestamp) > 10 * 60)
            {
                return Content("无效访问");
            }
            User user = FindUserById(model.mId);
            if (user == null)
            {
                return Content("非法访问");
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", model.user);
            dict.Add("mId", model.mId);
            dict.Add("signatureKey", user.SecurityStamp);
            dict.Add("timestamp", model.timestamp);
            dict.Add("accessToken", accessToken);
            string sign = Singature.GetSignature(dict);
            if (sign.Equals(model.sign) == false)
            {
                return Content("无效签名");
            }
            VerifyModel verifyModel = new VerifyModel { userName = model.user, mId = model.mId, accessToken = accessToken };
            Session["Model"] = verifyModel;
            return RedirectToAction("Verify");
        }
        public ActionResult Verify()
        {
            VerifyModel model = (VerifyModel)Session["Model"];
            var mUser = FindUserById(model.mId);
            if (mUser == null)
            {
                return Content("用户信息不存在！");
            }
            //获取ReturnURL
            model.ReturnURL = GetReturnURLById(mUser.Id);
            string id = FindCustomUserByName(model.userName,mUser.Id);
            User uUser = FindUserById(id);
            if (uUser != null && uUser.OpenId != null && uUser.OpenId.Length != 0)
            {
                return View("Index", model);
            }
            else
            {
                Session["Model"] = model;
                return RedirectToAction("VerifySuccess",model);
            }
        }
        [HttpPost]
        public ActionResult Verify(VerifyModel verifyModel)
        {
            VerifyModel model = (VerifyModel)Session["Model"];
            string id = FindUserIdByName(model.userName);
            User user = FindUserById(id);
            if (user == null)
            {
                ModelState.AddModelError("code", "信息不匹配");
                return View("Index", model);
            }
            string key = GetLoginKey(user.Id, model.mId);
            if (key == null)
            {
                ModelState.AddModelError("code", "出现错误，请重试，重试多次无效请联系技术人员！");
                return View("Index", model);
            }
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            DateTime current = DateTime.Now.ToLocalTime();//DateTime.UtcNow for unix timestamp
            TimeSpan span = current - dt1970;
            int counter = (int)Math.Floor(span.TotalMilliseconds / (30 * 1000.0));
            double[] codeList = { GenerateCode.GenerateTwoFACode(key,counter-1),
            GenerateCode.GenerateTwoFACode(key,counter),
            GenerateCode.GenerateTwoFACode(key,counter+1)};
            foreach (var item in codeList)
            {
                if (item.Equals(verifyModel.code))
                {
                    return RedirectToAction("VerifySuccess");
                }
            }
            ModelState.AddModelError("code", "验证码错误，请重新输入！");
            return View("Index", model);
        }

        public ActionResult VerifySuccess()
        {
            VerifyModel model = (VerifyModel)Session["Model"];
            if (model.accessToken == null || model.accessToken.Length == 0)
            {
                return Content("非法访问");
            }
            return Redirect(model.ReturnURL + "?accessToken=" + model.accessToken);
        }
    }
}