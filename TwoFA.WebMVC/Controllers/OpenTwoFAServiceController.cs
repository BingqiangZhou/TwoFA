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
            //TempData["Url"]= HttpContext.Request.UrlReferrer.ToString();
            return RedirectToAction("Open");
        }
        public ActionResult Open()
        {
            //string url = (string)TempData["Url"];
            //ViewBag.UrlReferrer = url;
            //if (url.Equals("https://bingqiangzhou.cn/"))
            //{
            //    ViewBag.UrlReferrer = null;
            //}
            OpenTwoFAServiceModel model = (OpenTwoFAServiceModel)TempData["Model"];
            return View("Index", model);
        }
    }
}