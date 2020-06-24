using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebAPIApp
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi1",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            //   Route  GET method
            config.Routes.MapHttpRoute(
               name: "DefaultApi2",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { action = "get", id = RouteParameter.Optional }
            );

            //   Route  GET method
            config.Routes.MapHttpRoute(
               name: "DefaultApi3",
               routeTemplate: "api/{controller}/{action}/{latitude}/{longitude}/{radius}",
               defaults: new { action = "get", latitude = RouteParameter.Optional, longitude = RouteParameter.Optional, radius = RouteParameter.Optional }
            );
        }
    }

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "web/{controller}/{action}/{value}",
                defaults: new { controller = "UserEmail", action = "Index", value = UrlParameter.Optional }
            );
        }
    }
}