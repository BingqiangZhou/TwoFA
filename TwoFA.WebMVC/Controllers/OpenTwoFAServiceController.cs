using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class OpenTwoFAServiceController : TwoFAMVCController
    {
        [HttpPost]
        public ActionResult Index(OpenTwoFAServiceModel model)
        {
            return View("Index",model);
        }
        //public ActionResult Index(OpenTwoFAServiceModel model)
        //{
        //    return View("Index",model);
        //}
    }
}