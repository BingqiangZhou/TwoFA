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
        [HttpGet]
        public ActionResult Index(OpenTwoFAServiceModel model)
        {
            TempData["Model"] = model;
            return RedirectToAction("Open");
        }
        public ActionResult Open()
        {
            OpenTwoFAServiceModel model = (OpenTwoFAServiceModel)TempData["Model"];
            return View("Index", model);
        }
    }
}