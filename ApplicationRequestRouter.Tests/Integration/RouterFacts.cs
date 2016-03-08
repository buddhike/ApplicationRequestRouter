using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Microsoft.Owin.Hosting;
using Owin;
using Xunit;

namespace ApplicationRequestRouter.Tests.Integration
{
    public class RouterFacts : IDisposable
    {
        private readonly string _appServerUrl = "http://localhost:9000";
        private readonly string _middlewareUrl = "http://localhost:9001";

        private readonly IDisposable _appServer;
        private readonly IDisposable _middleware;

        public RouterFacts()
        {
            _appServer = WebApp.Start($"{_appServerUrl}", app =>
            {
                app.Run(async c =>
                {
                    c.Response.ContentType = "text/plain";
                    await c.Response.WriteAsync(c.Request.Path.Value);
                });
            });

            _middleware = WebApp.Start($"{_middlewareUrl}", app =>
            {
                app.UseApplicationRequestRouter(new RouterOptions
                {
                    Routes = new Dictionary<string, string>
                    {
                        {"/a", $"{_appServerUrl}"}
                    }
                });

                app.Run(async c =>
                {
                    c.Response.ContentType = "text/plain";
                    await c.Response.WriteAsync("unmatched");
                });
            });
        }

        [Fact]
        public async void ShouldRouteMappedRequests()
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{_middlewareUrl}/a");
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("/a", body);
        }

        [Fact]
        public async void ShouldNotRouteUnmappedRequests()
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{_middlewareUrl}/foo");
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("unmatched", body);
        }

        public void Dispose()
        {
            _appServer.Dispose();
            _middleware.Dispose();
        }
    }
}