using System.Net.Http;
using AzureDevOpsRest.Handlers;
using Flurl.Http.Configuration;

namespace AzureDevOpsRest
{
    public class HttpClientFactory : DefaultHttpClientFactory
    {
        // nested handlers dispose each other and outer handler is disposed by client
        #pragma warning disable CA2000 
        public override HttpClient CreateHttpClient(HttpMessageHandler handler) => 
            base.CreateHttpClient(new NotFoundHandler(new NonAuthoritativeInformationHandler(handler)));
        #pragma warning restore CA2000
    }
}