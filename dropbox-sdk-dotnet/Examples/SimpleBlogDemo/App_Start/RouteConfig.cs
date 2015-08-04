using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleBlogDemo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Blogs",
                url: "Blogs/{blogname}/{id}",
                defaults: new { controller = "Blogs", action = "Display", id = UrlParameter.Optional }
                );
            routes.MapRoute(
                name: "Edit",
                url: "Edit/{id}",
                defaults: new { controller = "Edit", action = "Index" }
                );
            routes.MapRoute(
                name: "Add",
                url: "Add",
                defaults: new { controller = "Edit", action = "Add" }
                );
            routes.MapRoute(
                name: "Preview",
                url: "Preview",
                defaults: new { controller = "Edit", action = "Preview" }
                );
            routes.MapRoute(
                name: "Article",
                url: "Article/{id}",
                defaults: new { controller = "Article", action = "Display" }
                );
             routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
       }
    }
}
