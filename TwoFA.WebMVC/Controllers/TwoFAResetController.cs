using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    public class TwoFAResetController : Controller
    {
        // GET: TwoFAReset
        public ActionResult Index(VerifyModel model)
        {
            TempData["Model"] = model;
            return RedirectToAction("Reset");
        }
        public ActionResult Reset()
        {
            VerifyModel model = (VerifyModel)TempData["Model"];
            return View("Index", model);
        }
    }
}