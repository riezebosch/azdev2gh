using System;
using FluentAssertions;
using Xunit;

namespace AzureDevOpsRest.Tests
{
    public static class RequestExtensionsTests
    {
        [Fact]
        public static void RequestArgumentNull() =>
            FluentActions
                .Invoking(() => ((IRequest<object>) null).WithQueryParams())
                .Should()
                .Throw<ArgumentNullException>();
        
        [Fact]
        public static void EnumerableRequestArgumentNull() =>
            FluentActions
                .Invoking(() => ((IEnumerableRequest<object>) null).WithQueryParams())
                .Should()
                .Throw<ArgumentNullException>();

        [Fact]
        public static void WithQueryParams() =>
            new TestRequest(string.Empty).WithQueryParams(("key", 1234))
                .QueryParams
                .Should()
                .Contain("key", 1234);
    }
}