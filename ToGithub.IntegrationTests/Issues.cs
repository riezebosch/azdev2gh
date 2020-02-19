using System.Threading.Tasks;
using FluentAssertions;
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
            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>())
                .As<IFromAzureDevOps>();
            await foreach (var id in source.ProductBacklogItems(_project.Name))
            {
                await _repository.GithubClient.Issue.Create(_repository.Repository.Id, await source.ToIssue(id));
            }
        }
    }
}