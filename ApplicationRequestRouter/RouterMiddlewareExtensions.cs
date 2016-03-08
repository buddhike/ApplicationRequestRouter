using System.Linq;
using System.Net;
using Owin;

namespace ApplicationRequestRouter
{
    public static class RouterMiddlewareExtensions
    {
        public static IAppBuilder UseApplicationRequestRouter(
            this IAppBuilder app, RouterOptions options)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePoints = int.MaxValue;

            var routeConfigs = from r in options.Routes
                select new RouteConfig(r.Key, r.Value);

            app.Use<RouterMiddleware>(
                new RouteTable(routeConfigs.ToArray()),
                new RouteHandler(
                    new StreamCopyOperation(),
                    new HttpRequestHeaderCopyOperation(),
                    new HttpResponseHeaderCopyOperation()));

            return app;
        } 
    }
}