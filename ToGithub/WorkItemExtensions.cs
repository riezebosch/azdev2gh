using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Octokit;

namespace ToGithub
{
    internal static class WorkItemExtensions
    {
        public static NewIssue ToIssue(this WorkItem item)
        {
            item.Fields.TryGetValue("System.Description", out string body);
            var issue = new NewIssue(item.Fields["System.Title"].ToString())
            {
                Body = body
            };
            return issue;
        }

        public static string ToTaskListItem(this WorkItem item) => (string) item.Fields["System.State"] switch
        {
            "Done" => $"- [x] {item.Fields["System.Title"]}",
            "In Progress" => $"- [ ] {item.Fields["System.Title"]}",
            "To Do" => $"- [ ] {item.Fields["System.Title"]}",
            var x  => throw new ArgumentException($"System.State: {x}")
        };
    }
}