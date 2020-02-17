using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Octokit;
using Xunit;

namespace ToGithub.Tests
{
    public static class IssueExtensionsTests
    {
        public static class Markdown
        {
            [Fact]
            public static void Convert()
            {
                const string html =
                    @"<div>Some <b>bold</b></div><div>and some <u>underline</u></div><div><i style="">cursive?</i></div><div><i style=""><br></i></div>
<h1><span style=""font-weight: normal;\"">Header1</span></h1><h2><span style=""font-weight: normal;"">Header2</span></h2><h3><span style=""font-weight: normal;"">Header3</span></h3><div><span style=""font-weight: normal;""><br></span></div>
<pre><code><div><span style=""font-weight: normal;"">public static void Main() {}</span></div></code></pre><div><span style=""font-weight: normal;""><br></span></div><div>
<span style=""font-weight: normal;""><br><span>😊</span></span></div><div><span style=""font-weight: normal;""><br></span></div><div><span style=""font-weight: normal;""><br></span></div>";

                new NewIssue(string.Empty)
                    {
                        Body = html
                    }
                    .ToMarkdown()
                    .Body
                    .Should()
                    .Be(@"
Some **bold**

and some <u>underline</u>



# Header1

## Header2

### Header3



```
public static void Main() {}
```



😊




");
            }

            [Fact]
            public static void ConvertNull() =>
                new NewIssue(string.Empty)
                    .ToMarkdown()
                    .Body
                    .Should()
                    .BeNull();
        }
    }

    public class TasksToList : IClassFixture<TestConfig>, IClassFixture<TemporaryTeamProject>
    {
        private readonly TemporaryTeamProject _project;
        private readonly VssConnection _connection;

        public TasksToList(TestConfig config, TemporaryTeamProject project)
        {
            _project = project;
            _connection = new VssConnection(new Uri($"https://dev.azure.com/{config.AzDo.Organization}"),
                new VssBasicCredential("", config.AzDo.Token));
        }

        [Fact]
        public async Task Do()
        {
            using var client = _connection.GetClient<WorkItemTrackingHttpClient>();
            var query = await client.QueryByWiqlAsync(new Wiql { Query = 
                $"SELECT [System.Id] FROM Workitems WHERE [System.TeamProject] = '{_project.Name}'" });

            query
                .WorkItems
                .Should()
                .HaveCount(2);
        }
    }
}