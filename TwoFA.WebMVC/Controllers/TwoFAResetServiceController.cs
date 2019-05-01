using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.Utils.ToolsClass;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.Models.Model;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class TwoFAResetServiceController : TwoFAMVCController
    {
        [HttpGet]
        public ActionResult Index(SignatureModel model)
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
            string sign = Singature.GetSignature(dict);
            if (sign.Equals(model.sign) == false)
            {
                return Content("无效签名");
            }
            VerifyModel verifyModel = new VerifyModel { userName = model.user, mId = model.mId };
            Session["Model"] = verifyModel;
            return RedirectToAction("Reset");
        }

        public ActionResult Reset()
        {
            VerifyModel model = (VerifyModel)Session["Model"];
            return View("Index", model);
        }
        [HttpPost]
        public ActionResult Reset(VerifyModel verifyModel)
        {
            VerifyModel model = (VerifyModel)Session["Model"];
            string id = FindCustomUserByName(model.userName, model.mId);
            User user = FindUserById(id);
            if (user == null)
            {
                return Content("非法访问");
            }
            if (user.ResetKey.Equals(verifyModel.resetCode))
            {
                user.OpenId = null;
                bool res = UpdateUser(user);
                if (res == true)
                {
                    return View("ResetSucceed");
                }
                return Content("出现未知错误！");
            }
            else
            {
                ModelState.AddModelError("resetCode", "重置码错误，请输入正确的重置码！");
                return View("Index", model);
            }
        }
    }
}