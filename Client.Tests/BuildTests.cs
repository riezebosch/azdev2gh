using System.Threading.Tasks;
using AzureDevOpsRestClient.Data.Build;
using AzureDevOpsRestClient.Requests;
using FluentAssertions;
using Xunit;

namespace AzureDevOpsRestClient.Tests
{
    public class BuildTests
    {
        [Fact]
        public async Task Get()
        {
            var client = new Client("manuel");
            var result = await client.GetAsync(Build.Definition("packer-tasks", 27));

            result.Should().BeEquivalentTo(new Definition
            {
                Id = 27,
                Name = "riezebosch.vsts-tasks-packer"
            });
        }
    }
}