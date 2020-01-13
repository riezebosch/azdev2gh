namespace AzureDevOpsRestClient.Requests
{
    public class ReleaseManagementRequest<TData> : Request<TData>
    {
        public ReleaseManagementRequest(string resource, string api) : base(resource, api)
        {
        }

        public override string BaseUrl(string organization) => $"https://vsrm.dev.azure.com/{organization}/";
    }
}