using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class HomeController : TwoFAMVCController
    {
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Detail()
        {
            return View("twofa_detail");
        }
        
    }
}