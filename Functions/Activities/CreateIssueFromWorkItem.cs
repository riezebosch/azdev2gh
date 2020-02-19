using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Octokit;
using ToGithub;

namespace Functions.Activities
{
    public class CreateIssueFromWorkItem
    {
        private readonly WorkItemTrackingHttpClientBase _client;
        private readonly IGitHubClient _github;
        private readonly IResolveChildren _resolveChildren;

        public CreateIssueFromWorkItem(
            WorkItemTrackingHttpClientBase client,
            IGitHubClient github,
            IResolveChildren resolveChildren)
        {
            _client = client;
            _github = github;
            _resolveChildren = resolveChildren;
        }
        
        [FunctionName(nameof(CreateIssueFromWorkItem))]
        public  async Task Run([ActivityTrigger](int id, int repository) data)
        {
            var item = await _client.GetWorkItemAsync(data.id);
            await _github.Issue.Create(data.repository, item.ToIssue().ToMarkdown().AddTaskList(_resolveChildren.For(item).ToEnumerable()));
        }
    }
}