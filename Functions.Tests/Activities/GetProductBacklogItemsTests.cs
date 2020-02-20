using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Functions.Activities;
using ToGithub;
using Xunit;

namespace Functions.Tests.Activities
{
    public static class GetProductBacklogItemsTests
    {
        [Fact]
        public static void Test()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true});
            var source = fixture.Create<IFromAzureDevOps>();
            
            var function = new GetProductBacklogItems(source);
            function
                .Run(fixture.Create<string>())
                .Should()
                .NotBeEmpty();
        }
    }
}