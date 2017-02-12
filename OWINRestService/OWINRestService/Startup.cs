using Owin;
using System.Web.Http;

namespace OWINRestService 

{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.EnableCors();

            config.Routes.MapHttpRoute(
                    name: "MunicipalityApi",
                    routeTemplate: "api/{controller}/{municipality}",
                    defaults: new { municipality = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
            name: "TaxApi",
            routeTemplate: "api/{controller}/{tax}",
            defaults: new { tax = RouteParameter.Optional }
        );

            config.Routes.MapHttpRoute(
              name: "DateApi",
              routeTemplate: "api/{controller}/{date}",
              defaults: new { date = RouteParameter.Optional }
          );

           
            appBuilder.UseWebApi(config);
        }
    }
}