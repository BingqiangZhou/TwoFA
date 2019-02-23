using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.ViewModel;

namespace TwoFA.WebMVC.Controllers
{
    [Authorize]
    public class ConfigController : TwoFAMVCController
    {
        
        public async Task<ActionResult> Index()
        {
            ViewBag.Id = ConfigurationManager.AppSettings["Id"];
            ViewBag.Token = ConfigurationManager.AppSettings["Token"];
            var userName = HttpContext.User.Identity.Name;
            if (userName == null)
            {
                return Content("你还没有登录");
            }
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Content("用户不存在");
            }
            var claims = await UserManager.GetClaimsAsync(user.Id);
            if (claims == null)
            {
                return Content("用户声明不存在");
            }
            var url = "";
            foreach (var claim in claims)
            {
                if (claim.Type.Equals("ReturnUrl"))
                {
                    url = claim.Value;
                    break;
                }
            }
            return View("Index",new ConfigModel {userName =user.UserName,mId=user.Id,
                serviceIsOpen =(user.OpenID!=null&&user.OpenID.Length != 0?true:false),mUrl = url });
        }
        [HttpPost]
        public async Task<ActionResult> GetToken()
        {
            var result = "Error";
            var userName = HttpContext.User.Identity.Name;
            if (userName != null)
            { 
                var user = await UserManager.FindByNameAsync(userName);
                if (user != null)
                {
                    result = user.SecurityStamp;
                }
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<ActionResult> GetResetKey()
        {
            var result = "Error";
            var userName = HttpContext.User.Identity.Name;
            if (userName != null)
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user != null)
                {
                    result = user.ResetKey;
                }
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<ActionResult> SetReturnURL(string url)
        {
            var result = "Error";
            var userName = HttpContext.User.Identity.Name;
            if (userName != null)
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user != null)
                {
                    var claims = await UserManager.GetClaimsAsync(user.Id);
                    if (claims != null)
                    {
                        //找到声明"ReturnUrl"，删除声明
                        foreach (var claim in claims)
                        {
                            if (claim.Type.Equals("ReturnUrl"))
                            {
                                var removeResult = await UserManager.RemoveClaimAsync(user.Id, claim);
                                if (removeResult.Succeeded)
                                {
                                    break;
                                }
                            }
                        }
                        var addResult = await UserManager.AddClaimAsync(user.Id, new Claim("ReturnUrl", url));
                        if (addResult.Succeeded)
                        {
                            result = true.ToString();
                        }
                    }
                }
            }
            return Json(result);
        }
    }
}