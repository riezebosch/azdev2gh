using System.Collections.Generic;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Octokit;
using Xunit;

namespace ToGithub.Tests
{
    public class WorkItemTests 
    {
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
    }
}