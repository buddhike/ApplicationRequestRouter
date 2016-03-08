using System;
using System.Linq;
using Microsoft.Owin;

namespace ApplicationRequestRouter
{
    public interface IRouteTable
    {
        RouteConfig Resolve(IOwinRequest request);
    }

    public class RouteTable : IRouteTable
    {
        public static readonly RouteConfig  Unmatched = 
            new RouteConfig("", "router://unmatched");
         
        private readonly RouteConfig[] _configuration;

        public RouteTable(params RouteConfig[] configuration)
        {
            _configuration = configuration
                .OrderByDescending(c => c.Source.Value).ToArray();
        }

        public RouteConfig Resolve(IOwinRequest request)
        {
            var longestMatch = 
                from c in _configuration
                where request.Path.StartsWithSegments(c.Source)
                select c;

            return longestMatch.FirstOrDefault() ?? Unmatched;
        } 
    }
}