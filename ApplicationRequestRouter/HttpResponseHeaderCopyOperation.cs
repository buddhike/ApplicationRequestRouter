using System.Net;
using Microsoft.Owin;

namespace ApplicationRequestRouter
{
    public interface IHttpResponseHeaderCopyOperation
    {
        IOwinResponse Copy(WebResponse source, IOwinResponse destination);
    }

    public class HttpResponseHeaderCopyOperation : 
        IHttpResponseHeaderCopyOperation
    {
        public IOwinResponse Copy(WebResponse source, IOwinResponse destination)
        {
            foreach (var key in source.Headers.AllKeys)
            {
                destination.Headers.Add(key, source.Headers.GetValues(key));
            }

            return destination;
        }
    }
}