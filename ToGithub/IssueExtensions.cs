using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Octokit;
using ReverseMarkdown;

namespace ToGithub
{
    public static class IssueExtensions
    {
        public static NewIssue ToMarkdown(this NewIssue issue)
        {
            issue.Body =  issue.Body != null
                ? new Converter
                {
                    Config =
                    {
                        GithubFlavored = true,
                        RemoveComments = true,
                        UnknownTags = Config.UnknownTagsOption.PassThrough
                    }
                }.Convert(issue.Body)
                : null;

            return issue;
        }
        
        public static NewIssue AddTaskList(this NewIssue issue, IEnumerable<WorkItem> children)
        {
            var sb = new StringBuilder(issue.Body);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendJoin(Environment.NewLine, children.Select(x => x.ToTaskListItem()));
            issue.Body = sb.ToString();
            
            return issue;
        }
    }
}