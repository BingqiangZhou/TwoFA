using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class TwoFAValidationServiceController : TwoFAMVCController
    {
        // GET: TwoFAValidationService
        public ActionResult Index(VerifyModel model)
        {
            TempData["Model"] = model;
            return RedirectToAction("Verify");
        }
        public ActionResult Verify()
        {
            VerifyModel model = (VerifyModel)TempData["Model"];
            var mUser = FindUserById(model.mId);
            if (mUser == null)
            {
                return Content("用户信息不存在！");
            }
            //获取ReturnURL
            model.ReturnURL = GetReturnURLById(mUser.Id);
            //model.userName + "_" + model.mId.Replace('-', '_')
            var userName = EncodeUserName(model.mId,model.userName);

            var uUser = FindUserByUserName(userName);
            if (uUser != null && uUser.OpenID != null && uUser.OpenID.Length != 0)
            {
                return View("Index", model);
            }
            else
            {
                return RedirectToAction("VerifySuccess",model);
            }
        }
        public ActionResult VerifySuccess(VerifyModel model)
        {
            TempData["Model"] = model;
            return RedirectToAction("ReturnUserPage");
        }
        public ActionResult ReturnUserPage()
        {
            VerifyModel model = (VerifyModel)TempData["Model"];
            return Redirect(model.ReturnURL + "?accessToken=" + HttpUtility.UrlEncode(model.accessToken));
        }
    }
}