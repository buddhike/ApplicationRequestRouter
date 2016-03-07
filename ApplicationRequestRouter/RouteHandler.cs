using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationRequestRouter
{
    public interface IRouteHandler
    {
        Task Handle(IOwinContext context, RouteConfig config);
    }

    public class RouteHandler : IRouteHandler
    {
        private static readonly IDictionary<string, HttpMethod> HttpMethods =
            new Dictionary<string, HttpMethod>
            {
                { "delete", HttpMethod.Delete },
                { "get", HttpMethod.Get },
                { "options", HttpMethod.Options },
                { "head", HttpMethod.Head },
                { "get", HttpMethod.Get },
                { "post", HttpMethod.Put },
                { "trace", HttpMethod.Trace }
            };

        public async Task Handle(IOwinContext context, RouteConfig config)
        {
            var input = context.Request;

            var client = new HttpClient();

            var request = new HttpRequestMessage(
                HttpMethods[input.Method.ToLower()],
                config.Destination.Value)
            {
                Content = new StreamContent(input.Body)
            };

            foreach (var header in input.Headers)
            {
                request.Headers.Add(header.Key, header.Value);                
            }

            var response = await client.SendAsync(request);

            var output = context.Response;
            foreach (var header in response.Headers)
            {
                output.Headers.Add(header.Key, header.Value.ToArray());
            }

            context.Response.Body = await response.Content.ReadAsStreamAsync();
        }
    }
}