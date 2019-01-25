using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TwoFA.WebMVC.Models.Infrastructure;

namespace TwoFA.MiniProgramApi.Controllers
{
    public class AccountController : ApiController
    {
        public string Add(string userOpenId,string manufactruerId,string token)
        {

            return "hello";
        }
    }
}
