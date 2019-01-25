using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TwoFA.WebMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //移除WebFrom视图引擎
            RemoveWebFormEngines();
            //注册域
            AreaRegistration.RegisterAllAreas();
            //注册路由
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //注册捆绑包
            BundleConfig.RegisterStyleBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// 移除WebForm视图引擎，不再搜索asp文件
        /// </summary>
        private void RemoveWebFormEngines()
        {
            var viewEngines = ViewEngines.Engines;
            var webFromViewEngines =  viewEngines.OfType<WebFormViewEngine>().FirstOrDefault();

            if (webFromViewEngines != null)
            {
                viewEngines.Remove(webFromViewEngines);
            }
        }
    }
}
