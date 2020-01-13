using AzureDevOpsRestClient.Data.Release;

namespace AzureDevOpsRestClient.Requests
{
    public static class Release
    {
        private class Request<TData> : Requests.Request<TData>
        {
            public Request(string resource, string api) : base(resource, api)
            {
            }

            public override string BaseUrl(string organization) => $"https://vsrm.dev.azure.com/{organization}/";
        }
        
        public static IRequest<Definition> Definition(string project, int id)
            => new Request<Definition>($"{project}/_apis/release/definitions/{id}", "5.1");
    }
}