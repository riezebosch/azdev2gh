
using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Octokit;
using Xunit;

namespace ToGithub.Tests
{
    
    public class Tests
    {
        private TestConfig _config = new TestConfig();


        [Fact]
        public async Task GetWorkItemIds()
        {
            var connection = new VssConnection(new Uri($"https://dev.azure.com/{_config.Organization}"), new VssBasicCredential("",_config.Token));
            var client = connection.GetClient<WorkItemTrackingHttpClient>();
        
            var wiql = new Wiql()
            {
                Query = "Select [Id], [State], [Title] " +
                        "From WorkItems"
                
            };
            var result = await client.QueryByWiqlAsync(wiql);
        
            result.WorkItems.Should().NotBeEmpty();
            var item = await client.GetWorkItemAsync(result.WorkItems.First().Id, new []{"System.Id", "System.Title", "System.Description"});

            item.Fields.Should().ContainKeys("System.Title", "System.Description");
        }


        [Fact] 
        public async Task CreateIssueFromWorkItem()
        {
            var connection = new VssConnection(new Uri($"https://dev.azure.com/{_config.Organization}"), new VssBasicCredential("",_config.Token));
            var azdo = connection.GetClient<WorkItemTrackingHttpClient>();
        
            var wiql = new Wiql()
            {
                Query = "Select [Id], [State], [Title] " +
                        "From WorkItems " +
                        "Order by [Id] Desc"
                
            };
            
            var result = await azdo.QueryByWiqlAsync(wiql);

            var githubClient = new GitHubClient(new ProductHeaderValue("my-cool-app"))
            {
                Credentials = new Credentials(_config.GithubToken)
            };



            foreach (var workItem in result.WorkItems.Take(5))
            {
                var item = await azdo.GetWorkItemAsync(workItem.Id,new []{"System.Id", "System.Title", "System.Description"});
                item.Fields.TryGetValue("System.Description", out string body);
                    
                await githubClient.Issue.Create("riezebosch", "azure-devops-migration",
                    new NewIssue(item.Fields["System.Title"].ToString())
                    {
                        Body =  body
                    });
            }
            
            
        }
        
        
    }
    
    
}