using System;
using Microsoft.Owin;

namespace ApplicationRequestRouter
{
    public class RouteConfig
    {
        public RouteConfig(string source, string destination)
            : this(new PathString(source), new Uri(destination))
        {
        }

        public RouteConfig(PathString source, Uri destination)
        {
            Source = source;
            Destination = destination;
        }

        public PathString Source { get; private set; }

        public Uri Destination { get; private set; }
    }
}