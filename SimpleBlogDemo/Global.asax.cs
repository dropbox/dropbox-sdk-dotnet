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
using WebMatrix.WebData;

namespace SimpleBlogDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string AppKey { get; private set; }

        public static string AppSecret { get; private set; }

        protected void Application_Start()
        {
            InitializeAppKeyAndSecret();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var store = new SimpleBlogDemo.Models.UsersStore();
            if (store.Users.Count() == 0)
            {
                store.Users.Add(new Models.UserProfile { UserName = "Admin" });
                store.SaveChanges();
            }

            WebSecurity.InitializeDatabaseConnection("UsersStore", "UserProfiles", "ID", "UserName", true);
        }

        private void InitializeAppKeyAndSecret()
        {
            var appKey = WebConfigurationManager.AppSettings["DropboxAppKey"];
            var appSecret = WebConfigurationManager.AppSettings["DropboxAppSecret"];

            if (string.IsNullOrWhiteSpace(appKey) ||
                string.IsNullOrWhiteSpace(appSecret))
            {
                var infoPath = HttpContext.Current.Server.MapPath("~/App_Data/DropboxInfo.json");

                if (File.Exists(infoPath))
                {
                    string json;

                    using (var stream = new FileStream(infoPath, FileMode.Open, FileAccess.Read))
                    {
                        var reader = (TextReader)new StreamReader(stream);
                        json = reader.ReadToEnd();
                    }
                    var ser = new JavaScriptSerializer();
                    var info = ser.Deserialize<Dictionary<string, string>>(json);

                    appKey = info["AppKey"];
                    appSecret = info["AppSecret"];
                }
            }

            AppKey = appKey;
            AppSecret = appSecret;
        }
    }
}
