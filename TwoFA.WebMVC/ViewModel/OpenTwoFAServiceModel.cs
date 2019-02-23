using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebMVC.ViewModel
{
    public class OpenTwoFAServiceModel
    {
        public string userName { get; set; }
        public string mId { get; set; }
        public string token { get; set; }
    }
}