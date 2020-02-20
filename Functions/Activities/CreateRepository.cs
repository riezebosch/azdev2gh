using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Octokit;

namespace Functions.Activities
{
    public class CreateRepository
    {
        private readonly Func<string, IGitHubClient> _github;

        public CreateRepository(Func<string, IGitHubClient> github) => 
            _github = github;

        public async Task<long> Run([ActivityTrigger] string token)
        {
            var client = _github(token);
            var repository = await client.Repository.Create(new NewRepository(Guid.NewGuid().ToString().Substring(0, 8)));
            return repository.Id;
        }
    }
}