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
        public async Task<ActionResult> Verify()
        {
            VerifyModel model = (VerifyModel)TempData["Model"];
            var userName = model.userName + "_" + model.mId.Replace('-', '_'); ;
            var user = await UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                var claims = await UserManager.GetClaimsAsync(model.mId);
                if (claims != null)
                {
                    foreach (var claim in claims)
                    {
                        if (claim.Type.Equals("ReturnUrl"))
                        {
                            model.ReturnURL = claim.Value;
                            break;
                        }
                    }
                }
            }
            else
            {
                return Content("用户信息不存在！");
            }
            if (user.OpenID != null && user.OpenID.Length != 0)
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