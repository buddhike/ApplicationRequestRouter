using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
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
        private readonly IStreamCopyOperation _streamCopyOperation;

        private readonly IHttpRequestHeaderCopyOperation 
            _requestHeaderCopyOperation;

        private static readonly string[] BodylessMethods =
        {"GET", "HEAD", "TRACE"};

        public RouteHandler(IStreamCopyOperation streamCopyOperation,
            IHttpRequestHeaderCopyOperation requestHeaderCopyOperation)
        {
            if (streamCopyOperation == null)
            {
                throw
                    new ArgumentNullException(nameof(streamCopyOperation));
            }

            if (requestHeaderCopyOperation == null)
            {
                throw
                    new ArgumentNullException(
                        nameof(requestHeaderCopyOperation));
            }

            _streamCopyOperation = streamCopyOperation;
            _requestHeaderCopyOperation = requestHeaderCopyOperation;
        }

        public async Task Handle(IOwinContext context, RouteConfig config)
        {
            var input = context.Request;
            var destination = new Uri(config.Destination, config.Source.Value);
            var request = WebRequest.CreateHttp(destination);
            request.Method = input.Method;

            _requestHeaderCopyOperation.Copy(input, request);

            if (!BodylessMethods.Contains(input.Method))
            {
                await _streamCopyOperation.Copy(input.Body,
                    request.GetRequestStream());
            }

            var response = await request.GetResponseAsync();
            var output = context.Response;

            for (var i = 0; i < response.Headers.Count; i++)
            {
                var k = response.Headers.GetKey(i);
                output.Headers.Add(k, response.Headers.GetValues(k));
            }

            var bodyStream = response.GetResponseStream();
            await _streamCopyOperation.Copy(bodyStream, output.Body);
        }
    }
}