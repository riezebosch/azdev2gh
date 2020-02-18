using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Octokit;

namespace ToGithub
{
    public static class WorkItemExtensions
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
    }
}