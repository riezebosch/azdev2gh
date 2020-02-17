using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Octokit;
using Xunit;
using Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation;

namespace ToGithub.Tests
{
    public class TemporaryTeamProject : IAsyncLifetime
    {
        public VssConnection Connection { get; }
        public string Name { get; } = Guid.NewGuid().ToString().Substring(0, 8);

        public TemporaryTeamProject()
        {
            var config = new TestConfig();
            Connection = new VssConnection(new Uri($"https://dev.azure.com/{config.AzDo.Organization}"), 
                new VssBasicCredential("", config.AzDo.Token));
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            await CreateProject(Name);

            var parent = await CreateProductBacklogItem(Name);
            var child = await CreateTask(Name);

            await CreateLink(parent, child);
        }
        
        private async Task CreateProject(string name)
        {
            var client = Connection.GetClient<ProjectHttpClient>();
            var operation = await client
                .QueueCreateProject(new TeamProject
                {
                    Name = name,
                    Description = "",
                    Visibility = ProjectVisibility.Public,
                    Capabilities = new Dictionary<string, Dictionary<string, string>>
                    {
                        ["versioncontrol"] = new Dictionary<string, string>
                        {
                            ["sourceControlType"] = "Git"
                        },
                        ["processTemplate"] = new Dictionary<string, string>
                        {
                            ["templateTypeId"] =  "6b724908-ef14-45cf-84f8-768b5384da45"   
                        }
                    }
                    
                });

            var operations = Connection.GetClient<OperationsHttpClient>();
            while (operation.Status == OperationStatus.Queued || operation.Status == OperationStatus.InProgress)
            {
                operation = await operations.GetOperationAsync(operation.Id);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
            
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        private async Task<WorkItem> CreateProductBacklogItem(string name)
        {
            var doc = new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Sample PBI"
                }
            };

            return await Connection.GetClient<WorkItemTrackingHttpClient>().CreateWorkItemAsync(doc, name, "Product Backlog Item");
        }
        
        private async Task<WorkItem> CreateTask(string name)
        {
            var doc = new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Sample task"
                }
            };

            return await Connection.GetClient<WorkItemTrackingHttpClient>().CreateWorkItemAsync(doc, name, "Task");
        }

        private async Task CreateLink(WorkItem parent, WorkItem child)
        {
            await Connection.GetClient<WorkItemTrackingHttpClient>().UpdateWorkItemAsync(new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "System.LinkTypes.Hierarchy-Forward",
                        url = child.Url
                    }
                }
            }, "test", parent.Id.Value);
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            var client = Connection.GetClient<ProjectHttpClient>();
            var project = await client.GetProject(Name);
            var operation = await client
                .QueueDeleteProject(project.Id);
            
            var operations = Connection.GetClient<OperationsHttpClient>();
            while (operation.Status == OperationStatus.Queued || operation.Status == OperationStatus.InProgress)
            {
                operation = await operations.GetOperationAsync(operation.Id);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            if (operation.Status == OperationStatus.Failed)
            {
                throw new Exception("Delete Team Project failed.");
            }
            
            Connection.Dispose();
        }
    }
}