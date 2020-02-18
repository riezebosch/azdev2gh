using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Octokit;

namespace ToGithub
{
    public static class WorkItemExtensions
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/devops/integrate/quickstarts/work-item-quickstart?view=azure-devops#c-code-snippet
        /// </summary>
        public static async IAsyncEnumerable<WorkItem> GetWorkItems(this WorkItemTrackingHttpClient client, string name, params string[] fields)
        {
            var result = await client.QueryByWiqlAsync(new Wiql { Query = $"Select [System.Id] From WorkItems WHERE [System.TeamProject] = '{name}'" });
            foreach (var item in result.WorkItems)
            {
                yield return await client.GetWorkItemAsync(item.Id, fields);
            }
        }

        public static NewIssue ToIssue(this WorkItem item)
        {
            item.Fields.TryGetValue("System.Description", out string body);
            var issue = new NewIssue(item.Fields["System.Title"].ToString())
            {
                Body = body
            };
            return issue;
        }

        public static async IAsyncEnumerable<WorkItemLink> HierarchyForwardRelatedItems(this WorkItemTrackingHttpClientBase client, WorkItem parent)
        {
            var result = await client.QueryByWiqlAsync(new Wiql
            {
                Query = $@"
SELECT [System.Id]
FROM WorkItemLinks
WHERE ([Source].[System.Id] IN ({parent.Id})) 
AND ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') "
            });

            foreach (var item in result.WorkItemRelations)
            {
                yield return item;
            }
        }
    }
}