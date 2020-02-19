using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Octokit;
using ReverseMarkdown;
using ReverseMarkdown.Converters;

namespace ToGithub
{
    internal static class IssueExtensions
    {
        public static NewIssue ToMarkdown(this NewIssue issue)
        {
            var converter = new Converter
            {
                Config =
                {
                    GithubFlavored = true,
                    RemoveComments = true,
                    UnknownTags = Config.UnknownTagsOption.PassThrough, 
                    TableWithoutHeaderRowHandling = Config.TableWithoutHeaderRowHandlingOption.EmptyRow
                }
            };
            converter.Register("style", new Drop(converter));
            issue.Body =  issue.Body != null
                ? converter.Convert(issue.Body).Trim('\r', '\n', '\t', ' ')
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