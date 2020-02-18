using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Xunit;
using Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation;

namespace ToGithub.IntegrationTests
{
    public class TemporaryTeamProject : IAsyncLifetime
    {
        private const string Scrum = "6b724908-ef14-45cf-84f8-768b5384da45";
        public string Name { get; } = Guid.NewGuid().ToString().Substring(0, 8);
        public VssConnection Connection { get; }

        public TemporaryTeamProject()
        {
            var config = new TestConfig();
            Connection = new VssConnection(new Uri($"https://dev.azure.com/{config.AzDo.Organization}"), 
                new VssBasicCredential("", config.AzDo.Token));
        }

        private async Task CreateProject()
        {
            var client = Connection.GetClient<ProjectHttpClient>();
            var operation = await client
                .QueueCreateProject(new TeamProject
                {
                    Name = Name,
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
                            ["templateTypeId"] =  Scrum
                        }
                    }
                });

            await WaitForStatus(operation);
            ThrowIfFailed(operation);
        }

        public async Task<WorkItem> CreateProductBacklogItem(string title)
        {
            var doc = new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = title
                }
            };

            return await Connection.GetClient<WorkItemTrackingHttpClient>().CreateWorkItemAsync(doc, Name, "Product Backlog Item");
        }

        public async Task<WorkItem> CreateTask(string title)
        {
            var doc = new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = title
                }
            };

            return await Connection.GetClient<WorkItemTrackingHttpClient>().CreateWorkItemAsync(doc, Name, "Task");
        }

        public async Task CreateLink(WorkItem parent, WorkItem child)
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

        private async Task DeleteTeamProject()
        {
            var operation = await QueueDeleteRetry();
            await WaitForStatus(operation);
            ThrowIfFailed(operation);
        }

        private async Task WaitForStatus(OperationReference operation)
        {
            var operations = Connection.GetClient<OperationsHttpClient>();
            while (operation.Status == OperationStatus.Queued ||
                   operation.Status == OperationStatus.InProgress ||
                   operation.Status == OperationStatus.NotSet)
            {
                operation = await operations.GetOperationAsync(operation.Id);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task<OperationReference> QueueDeleteRetry()
        {
            var client = Connection.GetClient<ProjectHttpClient>();
            var project = await client.GetProject(Name);

            while (true)
            {
                try
                {
                    return await client.QueueDeleteProject(project.Id);
                }
                catch (ProjectWorkPendingException)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        private static void ThrowIfFailed(OperationReference operation)
        {
            if (operation.Status == OperationStatus.Failed)
            {
                throw new Exception("Operation failed.");
            }
        }

        async Task IAsyncLifetime.InitializeAsync() => await CreateProject();

        async Task IAsyncLifetime.DisposeAsync()
        {
            await DeleteTeamProject();
            Connection.Dispose();
        }
    }
}