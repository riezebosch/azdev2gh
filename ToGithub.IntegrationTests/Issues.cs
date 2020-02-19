using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Xunit;

namespace ToGithub.IntegrationTests
{
    public class Tests : IClassFixture<TemporaryTeamProject>, IClassFixture<TemporaryRepository>
    {
        private readonly TemporaryTeamProject _project;
        private readonly TemporaryRepository _repository;

        public Tests(TemporaryTeamProject project, TemporaryRepository repository)
        {
            _project = project;
            _repository = repository;
        }

        [Fact] 
        public async Task CreateIssueFromWorkItem()
        {
            using var client = _project.Connection.GetClient<WorkItemTrackingHttpClient>();
            var source = new FromAzureDevOps(client);
            await foreach (var item in source.ProductBacklogItems(_project.Name, "System.Id", "System.Title", "System.Description"))
            {
                var issue = item
                    .ToIssue()
                    .ToMarkdown()
                    .AddTaskList(source.ChildrenFor(item).ToEnumerable());
                await _repository.GithubClient.Issue.Create(_repository.Repository.Id, issue);
            }
        }
    }
}