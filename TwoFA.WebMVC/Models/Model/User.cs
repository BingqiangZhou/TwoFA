using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFA.WebMVC.Models.Model
{
    public class User : IdentityUser
    {
        public string OpenID { get; set; }
        //public string Key { get; set; }
    }
}
