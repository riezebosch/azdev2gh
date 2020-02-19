using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace ToGithub
{
    public class FromAzureDevOps
    {
        private readonly WorkItemTrackingHttpClientBase _client;

        public FromAzureDevOps(WorkItemTrackingHttpClientBase client) => _client = client;

        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/devops/integrate/quickstarts/work-item-quickstart?view=azure-devops#c-code-snippet
        /// </summary>
        public async IAsyncEnumerable<WorkItem> ProductBacklogItems(string area, params string[] fields)
        {
            var result = await _client.QueryByWiqlAsync(new Wiql { Query = $"Select [System.Id] From WorkItems WHERE [System.AreaPath] UNDER '{area}' AND [System.WorkItemType] = 'Product Backlog Item'" });
            foreach (var item in result.WorkItems)
            {
                yield return await _client.GetWorkItemAsync(item.Id, fields);
            }
        }

        public async IAsyncEnumerable<WorkItem> ChildrenFor(WorkItem parent)
        {
            var result = await _client.QueryByWiqlAsync(new Wiql
            {
                Query = QueryStolenFromAzureDevOps(parent)
            });

            if (!result.WorkItemRelations.Any())
            {
                yield break;;
            }
            
            foreach (var item in await _client.GetWorkItemsAsync(
                from x in result.WorkItemRelations
                where x.Rel != null // remove self reference to parent
                select x.Target.Id, new []{ "System.Title", "System.State"}))
            {
                yield return item;
            }
        }

        private static string QueryStolenFromAzureDevOps(WorkItem parent) =>
            $@"
SELECT [System.Id]
FROM WorkItemLinks
WHERE ([Source].[System.Id] IN ({parent.Id})) 
AND ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward')";
    }
}