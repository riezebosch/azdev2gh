using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace AzureDevOpsRest
{
    public class Client : IClient
    {
        private readonly string _organization;
        private readonly string _token;
        
        public Client(string organization) => 
            _organization = organization;
        public Client(string organization, string token) : this(organization) => 
            _token = Validate(token);
        static Client() => 
            FlurlHttp.Configure(settings => settings.HttpClientFactory = new HttpClientFactory());

        public Task<TData> GetAsync<TData>(IRequest<TData> request) =>
            Request(request).GetJsonAsync<TData>();
        
        public IAsyncEnumerable<TData> GetAsync<TData>(IEnumerableRequest<TData> request) =>
            request.Enumerator(Request(request));

        private IFlurlRequest Request<TData>(IRequest<TData> request) =>
            new Url(request.BaseUrl(_organization))
                .AppendPathSegment(request.Resource)
                .SetQueryParams(request.QueryParams)
                .WithBasicAuth(string.Empty, _token);

        private static string Validate(string token)
        {
            if (!string.IsNullOrEmpty(token) && token.Length != 52)
                throw new ArgumentException(
                    "Token is expected to be null or empty for public projects or having length of 52 for private projects",
                    nameof(token));

            return token;
        }
    }
}