using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Octokit;

namespace Functions.Activities
{
    public class CreateRepository
    {
        private readonly Func<GitHubData, IGitHubClient> _target;

        public CreateRepository(Func<GitHubData, IGitHubClient> target) => 
            _target = target;

        [FunctionName(nameof(CreateRepository))]
        public async Task<long> Run([ActivityTrigger] GitHubData input)
        {
            var client = _target(input);
            var repository = await client.Repository.Create(new NewRepository(Guid.NewGuid().ToString().Substring(0, 8)));
            return repository.Id;
        }
    }
}