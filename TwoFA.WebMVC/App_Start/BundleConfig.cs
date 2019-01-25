using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace TwoFA.WebMVC
{
    public static class BundleConfig
    {
        public static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/*.css"));
            bundles.Add(new ScriptBundle("~/Scripts/js")
                .Include("~/Scripts/jquery-3.0.0.js", 
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.unobtrusive-ajax.js"));
        }
    }
}