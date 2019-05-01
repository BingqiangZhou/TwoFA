using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebMVC.ViewModel
{
    public class SignatureModel
    {
        public string user { get; set; }
        public string mId { get; set; }
        public string sign { get; set; }
        public string timestamp { get; set; }
    }
}