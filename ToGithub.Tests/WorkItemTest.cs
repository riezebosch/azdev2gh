using System;
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

        public static class ToTask
        {
            [Fact]
            public static void Done()
            {
                var item = new WorkItem
                {
                    Fields = new Dictionary<string, object>
                    {
                        ["System.Title"] = "title",
                        ["System.State"] = "Done"
                    }
                };

                item.ToTaskListItem()
                    .Should()
                    .Be("- [x] title");
            }

            [Fact]
            public static void ToDo()
            {
                var item = new WorkItem
                {
                    Fields = new Dictionary<string, object>
                    {
                        ["System.Title"] = "title",
                        ["System.State"] = "To Do"
                    }
                };

                item.ToTaskListItem()
                    .Should()
                    .Be("- [ ] title");
            }

            [Fact]
            public static void InProgress()
            {
                var item = new WorkItem
                {
                    Fields = new Dictionary<string, object>
                    {
                        ["System.Title"] = "title",
                        ["System.State"] = "In Progress"
                    }
                };

                item.ToTaskListItem()
                    .Should()
                    .Be("- [ ] title");
            }

            [Fact]
            public static void Unknown()
            {
                var item = new WorkItem
                {
                    Fields = new Dictionary<string, object>
                    {
                        ["System.Title"] = "title",
                        ["System.State"] = "a3rjasldfjaf"
                    }
                };

                item
                    .Invoking(x => x.ToTaskListItem())
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("System.State: a3rjasldfjaf");
            }
        }
    }
}