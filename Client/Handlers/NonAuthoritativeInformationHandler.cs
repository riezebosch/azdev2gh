using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDevOpsRest.Handlers
{
    public class NonAuthoritativeInformationHandler : DelegatingHandler
    {
        public NonAuthoritativeInformationHandler(HttpMessageHandler handler) : base(handler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(request, cancellationToken);
            if (result.StatusCode == HttpStatusCode.NonAuthoritativeInformation)
            {
                result.StatusCode = HttpStatusCode.Unauthorized;
            }
            
            return result;
        }
    }
}