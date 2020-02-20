using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Octokit;
using ToGithub;

namespace Functions.Activities
{
    public class CreateIssueFromWorkItem
    {
        private readonly Func<GitHubData, IGitHubClient> _target;
        private readonly Func<AzureDevOpsData, IFromAzureDevOps> _source;

        public CreateIssueFromWorkItem(Func<GitHubData, IGitHubClient> target,
            Func<AzureDevOpsData, IFromAzureDevOps> source)
        {
            _target = target;
            _source = source;
        }
        
        [FunctionName(nameof(CreateIssueFromWorkItem))]
        public  async Task Run([ActivityTrigger](int id, int repository, GitHubData github, AzureDevOpsData azdo) data)
        {
            var (id, repository, github, azdo) = data;
            
            var source = _source(azdo);
            var item = await source.ToIssue(id);

            var target = _target(github);
            await target.Issue.Create(repository, item);
        }
    }
}