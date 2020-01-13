using System;
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
            Validate(token);
            
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

        private static void Validate(string token)
        {
            if (!string.IsNullOrEmpty(token) && token.Length != 52)
                throw new ArgumentException(
                    "Token is expected to be null or empty for public projects or having length of 52 for private projects",
                    nameof(token));
        }
    }
}