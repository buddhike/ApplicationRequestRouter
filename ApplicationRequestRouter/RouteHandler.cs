using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                { "head", HttpMethod.Head },
                { "delete", HttpMethod.Delete },
                { "get", HttpMethod.Get },
                { "options", HttpMethod.Options },
                { "post", HttpMethod.Post},
                { "put", HttpMethod.Put },
                { "trace", HttpMethod.Trace }
            };

        private static readonly HttpMethod[] BodylessMethods =
            { HttpMethod.Get, HttpMethod.Options, HttpMethod.Trace };

        public async Task Handle(IOwinContext context, RouteConfig config)
        {
            var input = context.Request;

            var client = new HttpClient();
            var method = HttpMethods[input.Method.ToLower()];

            var request = new HttpRequestMessage(
                method,
                new Uri(config.Destination, config.Source.Value));

            if(!BodylessMethods.Contains(method))
            {
                request.Content = new StreamContent(input.Body);
            }

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

            var bodyStream = await response.Content.ReadAsStreamAsync();

            var buffer = new byte[4096];

            var count = await bodyStream.ReadAsync(buffer, 0, buffer.Length);
            while (count != 0)
            {
                await output.Body.WriteAsync(buffer, 0, count);
                count = await bodyStream.ReadAsync(buffer, 0, buffer.Length);
            }
        }
    }
}