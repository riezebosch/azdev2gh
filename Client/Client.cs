using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace AzureDevOpsRest
{
    public class Client : IClient
    {
        private readonly string _organization;
        private readonly PersonalAccessToken _token;

        public Client(string organization, PersonalAccessToken token)
        {
            _organization = organization;
            _token = token;
        }

        public Client(string organization) : this(organization, PersonalAccessToken.Empty) 
        {
        }

        static Client() => 
            FlurlHttp.Configure(settings => settings.HttpClientFactory = new HttpClientFactory());

        public Task<TData> GetAsync<TData>(IRequest<TData> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return Setup(request).AllowHttpStatus(HttpStatusCode.NotFound).GetJsonAsync<TData>();
        }

        public IAsyncEnumerable<TData> GetAsync<TData>(IEnumerableRequest<TData> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            return enumerable.Enumerator(Setup(enumerable.Request));
        }

        private IFlurlRequest Setup<TData>(IRequest<TData> request) =>
            new Url(request.BaseUrl(_organization))
                .AppendPathSegment(request.Resource)
                .SetQueryParams(request.QueryParams)
                .WithBasicAuth(string.Empty, _token);
    }
}