using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeoSit.Client.Web.Exceptions
{
    public class HttpDuplicateRecordException : Exception
    {
        public HttpDuplicateRecordException(string stringContent, string reasonPhrase)
        {
            var message = new HttpResponseMessage((HttpStatusCode)480)
            {
                Content = new StringContent(stringContent),
                ReasonPhrase = reasonPhrase
            };
            throw new HttpResponseException(message);
        }
    }
}