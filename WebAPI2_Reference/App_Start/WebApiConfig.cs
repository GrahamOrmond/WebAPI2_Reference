using WebAPI2_Reference.API.Logging;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using System.Web.Http.Tracing;
using WebApiThrottle;
using WebAPI2_Reference.API.Attributes;
using Microsoft.AspNet.Identity;

namespace WebAPI2_Reference
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Enforce HTTPS
            //config.Filters.Add(new RequireHttpsAttribute());

            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // serialize all request paramaters to CamelCase
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            // enable tracing
            var traceWriter = new SystemDiagnosticsTraceWriter()
            {
                IsVerbose = true
            };
            config.Services.Replace(typeof(ITraceWriter), traceWriter);
            config.EnableSystemDiagnosticsTracing();

            // add rate limiting
            config.MessageHandlers.Add(new ThrottlingHandler()
            {
                Policy = new ThrottlePolicy(perMinute: 100)
                {
                    IpThrottling = true,
                    ClientThrottling = true,
                },
                Repository = new CacheRepository(),
                Logger = new ThrottleLogger(traceWriter)
            });
        }
    }
}
