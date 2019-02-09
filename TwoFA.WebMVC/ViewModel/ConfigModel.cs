using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebMVC.ViewModel
{
    public class ConfigModel
    {
        public string userName { get; set; }
        public string mId { get; set; }
        public bool serviceIsOpen { get; set; }
        public string mUrl { get; set; }
    }
}