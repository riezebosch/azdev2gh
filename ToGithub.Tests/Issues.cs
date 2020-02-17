
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Xunit;

namespace ToGithub.Tests
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
            await foreach (var item in client.GetWorkItems(_project.Name, "System.Id", "System.Title", "System.Description"))
            {
                await _repository.GithubClient.Issue.Create(
                    _repository.Repository.Id, 
                    item.ToIssue().ToMarkdown());
            }
        }
    }
}