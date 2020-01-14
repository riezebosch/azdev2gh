using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsRest.Data.Build;
using FluentAssertions;
using Xunit;

namespace AzureDevOpsRest.Requests.Tests
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
        
        [Fact]
        public void List()
        {
            var client = new Client("manuel");
            client
                .GetAsync(Build.Builds("packer-tasks").WithQueryParams(("$top", 2)))
                .ToEnumerable()
                .Count()
                .Should()
                .BeGreaterThan(4);
        }
    }
}