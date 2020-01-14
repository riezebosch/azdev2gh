using System.Net.Http;
using AzureDevOpsRest.Handlers;
using Flurl.Http.Configuration;

namespace AzureDevOpsRest
{
    public class HttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpClient CreateHttpClient(HttpMessageHandler handler) => 
            base.CreateHttpClient(new NotFoundHandler(new NonAuthoritativeInformationHandler(handler)));
    }
}