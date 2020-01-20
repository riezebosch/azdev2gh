using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using NSubstitute;
using Xunit;

namespace OfficialSdk.Tests
{
    public class UnitTest1 : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public UnitTest1(TestConfig config) => _config = config;

        [Fact]
        public async Task Test1()
        {
            var connection = new VssConnection(new Uri($"https://dev.azure.com/{_config.Organization}"), new VssBasicCredential("", _config.Token));
            var client = connection.GetClient<BuildHttpClient>();
            
            var builds = await client.GetBuildsAsync(project: "packer-tasks");
            builds.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task Stub()
        {
            var client = Substitute.For<BuildHttpClient>(null, null);
            client.GetBuildsAsync(project: "packer-tasks").Returns(new List<Build> { new Build() });
            
            var builds = await client.GetBuildsAsync(project: "packer-tasks");
            builds.Should().NotBeEmpty();
        }
    }
}