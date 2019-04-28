using Exhibition.Context;
using Exhibition.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exhibition.Controllers
{
    public class HomeController : Controller
    {
        private ExhibitionDbContext db = new ExhibitionDbContext();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            if (true)
            {
                var u = db.users.AsEnumerable().Where(m => m.Name == user.Name && m.Password == user.Password)
                     .Select(m => new User { Id = m.Id, Name = m.Name, Password = m.Password }).First();
                if (u != null)
                {
                    return Redirect("https://bingqiangzhou.cn/TwoFAValidationService?userName="
                        + u.Id + "&mId=" + ConfigurationManager.AppSettings["Id"]
                        + "&token=" + ConfigurationManager.AppSettings["Token"] + "&accessToken=888888");
                }
            }
            return View("账号或者密码错误");
        }
        public ActionResult LoginSuccess(string assessToken)
        {
            if (assessToken=="888888")
            {
                return Content("accessToken不对");
            }
            return View();
        }
        public ActionResult Open()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Open(User user)
        {
            if (true)
            {
                var u = db.users.AsEnumerable().Where(m => m.Name == user.Name && m.Password == user.Password)
                     .Select(m => new User { Id = m.Id, Name = m.Name, Password = m.Password }).First();
                if (u != null)
                {
                    string url = "http://localhost:62233/OpenTwoFAService?userName="
                        + u.Id + "&mId=" + ConfigurationManager.AppSettings["Id"]
                        + "&token=" + ConfigurationManager.AppSettings["Token"];
                    //Response.Write("<script>window.open('"+url+"','_blank')</script>");
                    return Redirect(url);
                }
            }
            return View("账号或者密码错误");
        }
        public ActionResult ShowQRCode()
        {
            return View();
        }
    }
}