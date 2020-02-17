
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Octokit;
using Xunit;

namespace ToGithub.Tests
{
    public class WorkItemTests : IClassFixture<TestConfig>, IDisposable
    {
        private readonly VssConnection _connection;

        public WorkItemTests(TestConfig config) =>
            _connection = new VssConnection(new Uri($"https://dev.azure.com/{config.AzDo.Organization}"), 
                new VssBasicCredential("", config.AzDo.Token));

        [Fact]
        public void GetWorkItemsTest()
        {
            using var client = _connection.GetClient<WorkItemTrackingHttpClient>();
            var result = client.GetWorkItems("System.Id", "System.Title").ToEnumerable();

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

        [Fact]
        public void ToIssueTest()
        {
            var item = new WorkItem { Fields = new Dictionary<string, object>
                {
                    ["System.Title"] = "title",
                    ["System.Description"] = "description"
                }};

            item.ToIssue()
                .Should()
                .BeEquivalentTo(new NewIssue("title")
                {
                    Body = "description"
                });
        }

        public void Dispose() => _connection.Disconnect();
    }
}