using Microsoft.Owin;

namespace ApplicationRequestRouter
{
    public class RouteConfig
    {
        public RouteConfig(string source, string destination)
            : this(new PathString(source), new PathString(destination))
        {
        }

        public RouteConfig(PathString source, PathString destination)
        {
            Source = source;
            Destination = destination;
        }

        public PathString Source { get; private set; }

        public PathString Destination { get; private set; }
    }
}