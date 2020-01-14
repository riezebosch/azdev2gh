using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsRest.Data.Release;
using FluentAssertions;
using Xunit;

namespace AzureDevOpsRest.Requests.Tests
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
        
        [Fact]
        public void List()
        {
            var client = new Client("manuel");
            client
                .GetAsync(Release.Definitions("packer-tasks"))
                .ToEnumerable()
                .Should()
                .NotBeEmpty();
        }
    }
}