using AzureDevOpsRestClient.Data.Build;

namespace AzureDevOpsRestClient.Requests
{
    public class Release
    {
        public static Request<Definition> Definition(string project, int id)
            => new ReleaseManagementRequest<Definition>($"{project}/_apis/release/definitions/{id}", "5.1");
    }
}