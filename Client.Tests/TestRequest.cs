using System.Collections.Generic;

namespace AzureDevOpsRest.Tests
{
    internal class TestRequest : IRequest<object>
    {
        public TestRequest(string resource) => Resource = resource;

        public string Resource { get; }
        public IDictionary<string, object> QueryParams { get; } = new Dictionary<string, object>();
        public string BaseUrl(string organization) => $"https://dev.azure.com/{organization}";
    }
}