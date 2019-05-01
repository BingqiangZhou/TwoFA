using Exhibition.Context;
using Exhibition.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exhibition.Controllers
{
    public class NoTwoFAController : Controller
    {
        private ExhibitionDbContext db = new ExhibitionDbContext();
        // GET: NoTwoFA
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(User user)
        {
            var u = db.users.AsNoTracking().Where(m => m.Name == user.Name && m.Password == user.Password)
                    .FirstOrDefault();
            if (u != null)
            {
                return View("LoginSuccess");
            }
            ModelState.AddModelError("Password", "账号密码不正确！");
            return View("Index");
        }
    }
}