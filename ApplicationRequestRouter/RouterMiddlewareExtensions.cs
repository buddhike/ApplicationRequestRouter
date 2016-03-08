using System.Linq;
using Owin;

namespace ApplicationRequestRouter
{
    public static class RouterMiddlewareExtensions
    {
        public static IAppBuilder UseApplicationRequestRouter(
            this IAppBuilder app, RouterOptions options)
        {
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