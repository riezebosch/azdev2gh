using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace AzureDevOpsRestClient
{
    public class Client
    {
        private readonly string _organization;
        private readonly string _token;

        public Client(string organization, string token)
        {
            _organization = organization;
            _token = token;
        }

        public Task<TData> GetAsync<TData>(Request<TData> request) =>
            new Url(request.BaseUrl(_organization))
                .AppendPathSegment(request.Resource)
                .AllowHttpStatus()
                .WithHeaders(request.Headers)
                .WithHeader("Content-Type", "application/json")
                .WithBasicAuth("", _token)
                .GetJsonAsync<TData>();
    }
}