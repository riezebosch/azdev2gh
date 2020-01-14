using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace AzureDevOpsRest.Tests
{
    public class NotFound
    {
        [Fact]
        public async Task NotFound_ResultIsNull()
        {
            var client = new Client("manuel", "");
            var result = await client.GetAsync(new TestRequest($"packer-tasks/_apis/build/builds/451234"));

            result.Should().BeNull();
        }
    }
}