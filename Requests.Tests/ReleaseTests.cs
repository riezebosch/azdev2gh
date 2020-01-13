using System.Threading.Tasks;
using AzureDevOpsRestClient.Data.Release;
using FluentAssertions;
using Xunit;

namespace AzureDevOpsRestClient.Requests.Tests
{
    public class ReleaseTest
    {
        [Fact]
        public async Task Get()
        {
            var client = new Client("manuel");
            var result = await client.GetAsync(Release.Definition("packer-tasks", 1));

            result.Should().BeEquivalentTo(new Definition
            {
                Id = 1,
                Name = "marketplace"
            });
        }
    }
}