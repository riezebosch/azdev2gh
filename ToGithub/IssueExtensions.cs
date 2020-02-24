using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Octokit;

namespace ToGithub
{
    internal static class IssueExtensions
    {
        public static NewIssue ToMarkdown(this NewIssue issue)
        {
            issue.Body =  issue.Body?.ToMarkdown();
            return issue;
        }

        public static NewIssue AddTaskList(this NewIssue issue, IEnumerable<WorkItem> children)
        {
            var tasks = children.Select(x => x.ToTaskListItem()).ToList();
            if (!tasks.Any()) 
                return issue;
            
            var sb = new StringBuilder(issue.Body);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendJoin(Environment.NewLine, tasks);
            issue.Body = sb.ToString();

            return issue;
        }
    }
}