
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Octokit;
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
        public void GetWorkItemIds()
        {
            using var client = _connection.GetClient<WorkItemTrackingHttpClient>();
            var result = GetWorkItems(client, "System.Id", "System.Title").ToEnumerable();

            result
                .Should()
                .NotBeEmpty()
                .And
                .Subject
                .First()
                .Fields
                .Should()
                .ContainKeys("System.Title");
        }

        private static async IAsyncEnumerable<WorkItem> GetWorkItems(WorkItemTrackingHttpClient client, params string[] fields)
        {
            var result = await client.QueryByWiqlAsync(new Wiql { Query = "Select [System.Id] From WorkItems" });
            foreach (var item in result.WorkItems)
            {
                yield return await client.GetWorkItemAsync(item.Id, fields);
            }
        }
        
        [Fact] 
        public async Task CreateIssueFromWorkItem()
        {
            using var client = _connection.GetClient<WorkItemTrackingHttpClient>();
            foreach (var item in GetWorkItems(client, "System.Id", "System.Title", "System.Description").ToEnumerable().Take(5))
            {
                item.Fields.TryGetValue("System.Description", out string body);
                    
                await _repository.GithubClient.Issue.Create(_repository.Repository.Id,
                    new NewIssue(item.Fields["System.Title"].ToString())
                    {
                        Body =  body
                    });
            }
        }

        public void Dispose() => _connection.Disconnect();
    }
}