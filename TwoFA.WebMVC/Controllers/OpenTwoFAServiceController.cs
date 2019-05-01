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
    public class OpenTwoFAServiceController : TwoFAMVCController
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
            Session["Model"] = model;
            //TempData["Url"]= HttpContext.Request.UrlReferrer.ToString();
            return RedirectToAction("Open");
        }
        public ActionResult Open()
        {
            SignatureModel model = (SignatureModel)Session["Model"];
            if(model == null)
            {
                return Content("未知错误");
            }
            string resetKey;
            string base64String;
            bool res;
            //处理数据，生成二维码等数据
            User mUser = FindUserById(model.mId); 
            if (mUser.UserName.Equals("TwoFA") == false)
            {
                res = OpenForCustomUser(mUser, model.user,out resetKey,out base64String);
            }
            else
            {
                res = OpenForManufacturer(mUser, model.user, out resetKey, out base64String);
            }
            if (res == false)
            {
                return Content("发生错误");
            }
            ViewBag.resetKey = resetKey;
            ViewBag.base64String = base64String;
            return View("Index", model);
        }
        [NonAction]
        public bool OpenForCustomUser(User mUser, string userName,out string resetKey,out string base64String)
        {
            string twoFAToken = GenerateUserToken(mUser.Id);
            string userId = FindCustomUserByName(userName,mUser.Id);
            if (userId == null)
            {
                bool createRes = CreateCustomUser(userName, mUser.Id);
                if (createRes == false)
                {
                    resetKey = null;
                    base64String = null;
                    return false;
                }
                else
                {
                    userId = FindCustomUserByName(userName, mUser.Id);
                }
            }
            User user = FindUserById(userId);
            string twoFAKey = GenerateCode.GenerateSHA1(user.Id + twoFAToken);
            //将生成的sha-1值的前12位作为重置key
            resetKey = GenerateCode.GenerateSHA1(user.SecurityStamp + twoFAToken).Substring(0, 12);
            user.ResetKey = resetKey;
            bool updateRes = UpdateUser(user);
            SaveToDatabase();
            if (updateRes == false)
            {
                resetKey = null;
                base64String = null;
                return false;
            }
            //保存两步验证秘钥
            bool setKeyRes = SetTwoFAKeyById(user.Id, mUser.Id, twoFAKey);
            if (setKeyRes == false)
            {
                resetKey = null;
                base64String = null;
                return false;
            }
            //生成二维码
            base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(
                GenerateQRCodeByZxing.GenerateQRCodeToBitmap(twoFAKey + "|" + user.Name + "|" + mUser.Name, 256, 256, 0));
            return true;
        }
        [NonAction]
        public bool OpenForManufacturer(User mUser, string userName, out string resetKey, out string base64String)
        {
            string twoFAToken = GenerateUserToken(mUser.Id);
            string userId = FindCustomUserByName(userName,mUser.Id);
            if (userId == null)
            {
                resetKey = null;
                base64String = null;
                return false;
            }
            User user = FindUserById(userId);
            string twoFAKey = GenerateCode.GenerateSHA1(user.Id + twoFAToken);
            //将生成的sha-1值的前12位作为重置key
            resetKey = GenerateCode.GenerateSHA1(user.SecurityStamp + twoFAToken).Substring(0, 12);
            user.ResetKey = resetKey;
            SaveToDatabase();
            bool updateRes = UpdateUser(user);
            if (updateRes == false)
            {
                resetKey = null;
                base64String = null;
                return false;
            }
            //保存两步验证秘钥
            bool setKeyRes = SetTwoFAKeyById(user.Id, mUser.Id, twoFAKey);
            if (setKeyRes == false)
            {
                resetKey = null;
                base64String = null;
                return false;
            }
            //生成二维码
            base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(
                GenerateQRCodeByZxing.GenerateQRCodeToBitmap(twoFAKey + "|" + user.Name + "|" + mUser.Name, 256, 256, 0));
            return true;
        }
        [HttpPost]
        public ActionResult HaveAddSucceed()
        {
            string result = "No";
            SignatureModel model = (SignatureModel)Session["Model"];
            string id = FindCustomUserByName(model.user, model.mId);
            User user = FindUserById(id);
            if (user != null && user.OpenId != null && user.OpenId.Length > 0)
            {
                result = "Yes";
            }
            return Json(result);
        }
    }
}