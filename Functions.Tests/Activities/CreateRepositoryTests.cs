using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Functions.Activities;
using NSubstitute;
using Octokit;
using Xunit;

namespace Functions.Tests.Activities
{
    public static class CreateRepositoryTests
    {
        [Fact]
        public static async Task Test()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true});
            var github = fixture.Create<IGitHubClient>();

            var function = new CreateRepository(token => github);
            await function.Run(fixture.Create<string>());

            await github
                .Repository
                .Received()
                .Create(Arg.Any<NewRepository>());
        }
    }
}