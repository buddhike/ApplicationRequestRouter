using System;
using System.Linq;
using System.Net;
using Microsoft.Owin;

namespace ApplicationRequestRouter
{
    public interface IHttpRequestHeaderCopyOperation
    {
        HttpWebRequest Copy(IOwinRequest source,
            HttpWebRequest destination);
    }

    public class HttpRequestHeaderCopyOperation : IHttpRequestHeaderCopyOperation
    {
        const char Quote = (char) 34;

        public HttpWebRequest Copy(IOwinRequest source,
            HttpWebRequest destination)
        {
            var nonRestrictedHeaders =
                from h in source.Headers
                where WebHeaderCollection.IsRestricted(h.Key) == false
                select h;

            foreach (var h in nonRestrictedHeaders.ToArray())
            {
                var sanitisedValues = from v in h.Value
                    select QuoteIfNeeded(v);

                destination.Headers.Add(h.Key,
                    string.Join(",", sanitisedValues.ToArray()));
            }

            destination.Accept = source.Headers["Accept"];
            destination.ContentType = source.Headers["Content-Type"];

            long contentLength;
            if (long.TryParse(source.Headers["Content-Length"],
                out contentLength))
            {
                destination.ContentLength = contentLength;
            }

            DateTime date;
            if (DateTime.TryParse(source.Headers["Date"], out date))
                destination.Date = date;

            DateTime ifModifiedSince;
            if (DateTime.TryParse(source.Headers["If-Modified-Since"],
                out ifModifiedSince))
            {
                destination.IfModifiedSince = ifModifiedSince;
            }

            destination.Referer = source.Headers["Referer"];
            destination.TransferEncoding = source.Headers["Transfer-Encoding"];
            destination.UserAgent = source.Headers["User-Agent"];

            return destination;
        }

        private static string QuoteIfNeeded(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (!value.Contains(','))
                return value;

            if (value[0] != Quote || value[value.Length - 1] != Quote)
                return $"\"{value}\"";

            return value;
        }
    }
}