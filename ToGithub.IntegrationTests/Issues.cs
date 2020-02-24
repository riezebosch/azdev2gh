using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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
        
        [Fact]
        public async Task ToComments()
        {
            var client = _project.Connection.GetClient<WorkItemTrackingHttpClient>();
            var parent = await _project.CreateProductBacklogItem("Sample PBI");
            await client.AddCommentAsync(new CommentCreate { Text = "first comment" }, _project.Name, parent.Id.Value);
            await client.AddCommentAsync(new CommentCreate { Text = "second comment" }, _project.Name, parent.Id.Value);

            var source = new FromAzureDevOps(client)
                .As<IFromAzureDevOps>();

            var issue = await _repository.GithubClient.Issue.Create(_repository.Repository.Id, await source.ToIssue(parent.Id.Value));
            await foreach (var comment in source.ToComments(parent.Id.Value))
            {
                await _repository.GithubClient.Issue.Comment.Create(_repository.Repository.Id, issue.Number, comment);
            }

            var comments = await _repository.GithubClient.Issue.Comment.GetAllForIssue(_repository.Repository.Id, issue.Number);
            comments
                .Select(x => x.Body)
                .Should()
                .Equal("first comment", "second comment");
        }
    }
}