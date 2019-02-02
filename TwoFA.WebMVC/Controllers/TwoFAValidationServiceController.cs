using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class TwoFAValidationServiceController : TwoFAMVCController
    {
        // GET: TwoFAValidationService
        public ActionResult Index()
        {
            return View("Index",new VerifyModel { userName="BingqiangZhou",mId= "868ccbc9-eb5a-4b9f-947d-b60209b45cf2", token= "f4070ec4-a944-4a77-974f-76b69944309a" });
        }
    }
}