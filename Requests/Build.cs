using AzureDevOpsRest.Data.Release;

namespace AzureDevOpsRest.Requests
{
    public static class Build
    {
        public static IRequest<Definition> Definition(string project, int id) => 
            new Request<Definition>($"{project}/_apis/build/definitions/{id}", "5.1");

        public static IEnumerableRequest<Definition> Definitions(string project) => 
            new Request<Definition>($"{project}/_apis/build/definitions/", "5.1").AsEnumerable();

        public static IEnumerableRequest<Data.Build.Build> Builds(string project) =>
            new Request<Data.Build.Build>($"{project}/_apis/build/builds/", "5.1").AsEnumerable();
    }
}