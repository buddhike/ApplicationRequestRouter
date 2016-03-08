using Microsoft.Owin;
using NSubstitute;
using Xunit;

namespace ApplicationRequestRouter.Tests
{
    public class RouteTableFacts
    {
        private readonly IOwinRequest _request;
        private readonly string _target = "http://a.a/";

        public RouteTableFacts()
        {
            _request = Substitute.For<IOwinRequest>();
        }

        [Fact]
        public void ShouldMatchLongestRoute()
        {
            var table = new RouteTable(
                new RouteConfig("/a", "http://b.b/"), 
                new RouteConfig("/a/b", _target));

            _request.Path.Returns(new PathString("/a/b/c"));

            var result = table.Resolve(_request);

            Assert.Equal(_target, result.Destination.ToString());
        }

        [Fact]
        public void ShouldReturnUnmatchedForUnknownRoutes()
        {
            var table = new RouteTable(new RouteConfig("/a", _target));
            _request.Path.Returns(new PathString("/b"));

            var result = table.Resolve(_request);

            Assert.Equal(RouteTable.Unmatched, result);
        }
    }
}