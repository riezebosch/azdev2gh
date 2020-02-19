using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Octokit;
using ToGithub;

namespace Functions.Activities
{
    public class CreateIssueFromWorkItem
    {
        private readonly IGitHubClient _target;
        private readonly IFromAzureDevOps _source;

        public CreateIssueFromWorkItem(IGitHubClient target,
            IFromAzureDevOps source)
        {
            _target = target;
            _source = source;
        }
        
        [FunctionName(nameof(CreateIssueFromWorkItem))]
        public  async Task Run([ActivityTrigger](int id, int repository) data)
        {
            var item = await _source.ToIssue(data.id);
            await _target.Issue.Create(data.repository, item);
        }
    }
}