using System.Collections.Generic;

namespace AzureDevOpsRestClient
{
    public class Request<TData>
    {
        public IDictionary<string, object> Headers = new Dictionary<string, object>();
        public Request(string resource, string api)
        {
            Resource = resource;
            Headers["api-version"] = api;
        }

        public string Resource { get; }

        public virtual string BaseUrl(string organization) =>  $"https://dev.azure.com/{organization}/";
    }
}