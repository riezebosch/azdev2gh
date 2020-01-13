using System.Collections.Generic;

namespace AzureDevOpsRestClient.Requests
{
    public class Request<TData> : IRequest<TData>
    {
        public IDictionary<string, object> QueryParams { get; } = new Dictionary<string, object>();
        public Request(string resource, string api)
        {
            Resource = resource;
            QueryParams["api-version"] = api;
        }

        public string Resource { get; }

        public virtual string BaseUrl(string organization) =>  $"https://dev.azure.com/{organization}/";
    }
}