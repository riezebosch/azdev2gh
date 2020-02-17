
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Xunit;

namespace ToGithub.Tests
{
    public class Tests : IClassFixture<TestConfig>, IClassFixture<TemporaryRepository> , IDisposable
    {
        private readonly TemporaryRepository _repository;
        private readonly VssConnection _connection;

        public Tests(TestConfig config, TemporaryRepository repository)
        {
            _repository = repository;
            _connection = new VssConnection(new Uri($"https://dev.azure.com/{config.AzDo.Organization}"), 
                new VssBasicCredential("", config.AzDo.Token));
        }

        [Fact] 
        public async Task CreateIssueFromWorkItem()
        {
            using var client = _connection.GetClient<WorkItemTrackingHttpClient>();
            foreach (var item in client.GetWorkItems("System.Id", "System.Title", "System.Description").ToEnumerable().Take(75))
            {
                await _repository.GithubClient.Issue.Create(
                    _repository.Repository.Id, 
                    item.ToIssue().ToMarkdown());
            }
        }

        public void Dispose() => _connection.Disconnect();
    }
}