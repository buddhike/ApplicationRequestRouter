using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationRequestRouter
{
    public class RouterMiddleware : OwinMiddleware
    {
        private readonly IRouteTable _routeTable;
        private readonly IRouteHandler _routeHandler;

        public RouterMiddleware(OwinMiddleware next, 
            IRouteTable routeTable,
            IRouteHandler routeHandler)
            : base(next)
        {
            if (routeTable == null)
                throw new ArgumentNullException(nameof(routeTable));

            if (routeHandler == null)
                throw new ArgumentNullException(nameof(routeHandler));

            _routeTable = routeTable;
            _routeHandler = routeHandler;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var route = _routeTable.Resolve(context.Request);
            if (route == RouteTable.Unmatched)
                await Next.Invoke(context);
            else
                await _routeHandler.Handle(context, route);
        }
    }
}