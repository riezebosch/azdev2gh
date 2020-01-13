using AzureDevOpsRestClient.Data.Release;

namespace AzureDevOpsRestClient.Requests
{
    public static class Build
    {
        public static IRequest<Definition> Definition(string project, int id) => 
            new Request<Definition>($"{project}/_apis/build/definitions/{id}", "5.1");
    }
}