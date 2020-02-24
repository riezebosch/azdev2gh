using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Octokit;

namespace ToGithub
{
    public interface IFromAzureDevOps
    {
        IAsyncEnumerable<int> ProductBacklogItems(string area);
        Task<NewIssue> ToIssue(int id);
        IAsyncEnumerable<string> ToComments(int id);
    }

    public class FromAzureDevOps : IFromAzureDevOps
    {
        private readonly WorkItemTrackingHttpClientBase _client;

        public FromAzureDevOps(WorkItemTrackingHttpClientBase client) => _client = client;

        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/devops/integrate/quickstarts/work-item-quickstart?view=azure-devops#c-code-snippet
        /// </summary>
        async IAsyncEnumerable<int> IFromAzureDevOps.ProductBacklogItems(string area)
        {
            var result = await _client.QueryByWiqlAsync(new Wiql { Query = $"Select [System.Id] From WorkItems WHERE [System.AreaPath] UNDER '{area}' AND [System.WorkItemType] = 'Product Backlog Item' AND [System.State] NOT IN ('Done', 'Removed')" });
            foreach (var item in result.WorkItems)
            {
                yield return item.Id;
            }
        }
        
        async Task<NewIssue> IFromAzureDevOps.ToIssue(int id)
        {
            var item = await _client.GetWorkItemAsync(id);
            return item
                .ToIssue()
                .ToMarkdown()
                .AddTaskList(RelatedFor(id).ToEnumerable());
        }

        public async IAsyncEnumerable<string> ToComments(int id)
        {
            foreach (var comment in (await _client.GetCommentsAsync(id)).Comments)
            {
                yield return comment.ToComment();
            }
        }

        private async IAsyncEnumerable<WorkItem> RelatedFor(int id)
        {
            var result = await _client.QueryByWiqlAsync(new Wiql
            {
                Query = QueryStolenFromAzureDevOps(id)
            });

            if (!result.WorkItemRelations.Any())
            {
                yield break;
            }
            
            foreach (var item in await _client.GetWorkItemsAsync(
                from x in result.WorkItemRelations
                where x.Rel != null // remove self reference to parent
                select x.Target.Id, new []{ "System.Title", "System.State"}))
            {
                yield return item;
            }
        }

        private static string QueryStolenFromAzureDevOps(int id) =>
            $@"
SELECT [System.Id]
FROM WorkItemLinks
WHERE ([Source].[System.Id] IN ({id})) 
AND ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward')";
    }
}