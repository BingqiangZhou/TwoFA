﻿using System;
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
        public ActionResult Index(VerifyModel model)
        {
            TempData["Model"] = model;
            return RedirectToAction("Verify");
        }
        public ActionResult Verify()
        {
            VerifyModel model = (VerifyModel)TempData["Model"];
            return View("Index", model);
        }
    }
}