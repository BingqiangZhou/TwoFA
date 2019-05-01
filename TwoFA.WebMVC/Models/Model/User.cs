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
        public string Name { get; set; }
        public string OpenId { get; set; }
        public string ResetKey { get; set; }
        public string Manufacturer { get; set; }
    }
}
