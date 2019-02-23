using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwoFA.WebMVC.Controllers
{
    public class UserManualController : Controller
    {
        // GET: UserManual
        public ActionResult Index()
        {
            return View("Home");
        }
    }
}