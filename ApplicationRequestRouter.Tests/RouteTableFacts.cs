using Microsoft.Owin;
using NSubstitute;
using Xunit;

namespace ApplicationRequestRouter.Tests
{
    public class RouteTableFacts
    {
        private readonly IOwinRequest _request;

        public RouteTableFacts()
        {
            _request = Substitute.For<IOwinRequest>();
        }

        [Fact]
        public void ShouldMatchLongestRoute()
        {
            var table = new RouteTable(new RouteConfig("/a", "/x"), 
                new RouteConfig("/a/b", "/y"));

            _request.Path.Returns(new PathString("/a/b/c"));

            var result = table.Resolve(_request);

            Assert.Equal(new PathString("/y"), result.Destination);
        }

        [Fact]
        public void ShouldReturnUnmatchedForUnknownRoutes()
        {
            var table = new RouteTable(new RouteConfig("/a", "/x"));
            _request.Path.Returns(new PathString("/b"));

            var result = table.Resolve(_request);

            Assert.Equal(RouteTable.Unmatched, result);
        }
    }
}