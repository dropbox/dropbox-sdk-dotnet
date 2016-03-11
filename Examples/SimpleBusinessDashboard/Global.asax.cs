using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace SimpleBusinessDashboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string AppKey { get; private set; }

        public static string AppSecret { get; private set; }

        public static string AccessToken { get; set; }

        protected void Application_Start()
        {
            InitializeAppKeyAndSecret();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void InitializeAppKeyAndSecret()
        {
            AppKey = WebConfigurationManager.AppSettings["DropboxAppKey"];
            AppSecret = WebConfigurationManager.AppSettings["DropboxAppSecret"];
            AccessToken = "";
        }
    }
}
