using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Functions.Activities;
using NSubstitute;
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
            
            var function = new GetProductBacklogItems(data => source);
            function
                .Run(fixture.Create<AzureDevOpsData>())
                .Should()
                .NotBeEmpty();
        }
        
        [Fact]
        public static void Limit()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true});
            var source = fixture.Create<IFromAzureDevOps>();
            source
                .ProductBacklogItems(Arg.Any<string>())
                .Returns(Enumerable.Range(0, 1000).ToAsyncEnumerable());
            
            var function = new GetProductBacklogItems(data => source);
            function
                .Run(fixture.Create<AzureDevOpsData>())
                .Should()
                .HaveCount(50);
        }
    }
}