using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Infrastructure;

namespace TwoFA.WebMVC.Controllers
{
    public class OpenTwoFAServiceController : TwoFAMVCController
    {
        // GET: OpenTwoFAService
        public ActionResult Index()
        {
            return View();
        }
    }
}