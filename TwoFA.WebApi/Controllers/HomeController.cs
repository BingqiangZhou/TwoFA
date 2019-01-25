using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.Models.Model;

namespace TwoFA.WebApi.Controllers
{
    public class HomeController:TwoFAApiController
    {
        [HttpGet]
        public string Index()
        {
            IdentityResult result = UserManager.Create(new User { UserName="bingqiangzhou" });
            IEnumerable<Claim> claims = UserManager.GetClaims("9f4f340a-e4bb-4b0d-9286-db4f48046e98");
            return "hello";
        }
    }
}
