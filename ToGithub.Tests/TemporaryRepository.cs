using System;
using System.Threading.Tasks;
using Octokit;
using Xunit;

namespace ToGithub.Tests
{
    public class TemporaryRepository : IAsyncLifetime
    {
        public Repository Repository { get; private set; }
        public GitHubClient GithubClient { get; }

        public TemporaryRepository()
        {
            var config = new TestConfig();
            GithubClient = new GitHubClient(new ProductHeaderValue("azure-devops-github-migration"))
            {
                Credentials = new Credentials(config.GitHub.Token)
            };
        }

        async Task IAsyncLifetime.InitializeAsync() => 
            Repository = await GithubClient.Repository.Create(new NewRepository(Guid.NewGuid().ToString().Substring(0,8)));

        async Task IAsyncLifetime.DisposeAsync() => 
            await GithubClient.Repository.Delete(Repository.Id);
    }
}