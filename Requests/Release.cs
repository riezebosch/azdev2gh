using AzureDevOpsRest.Data.Release;

namespace AzureDevOpsRest.Requests
{
    public static class Release
    {
        private class Request<TData> : Requests.Request<TData>
        {
            public Request(string resource, string version) : base(resource, version)
            {
            }

            public override string BaseUrl(string organization) => $"https://vsrm.dev.azure.com/{organization}/";
        }
        
        public static IRequest<Definition> Definition(string project, int id)
            => new Request<Definition>($"{project}/_apis/release/definitions/{id}", "5.1");

        public static IEnumerableRequest<Definition> Definitions(string project)
            => new Request<Definition>($"{project}/_apis/release/definitions", "5.1").AsEnumerable();
    }
}