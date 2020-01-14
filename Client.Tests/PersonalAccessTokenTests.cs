using System;
using FluentAssertions;
using Xunit;

namespace AzureDevOpsRest.Tests
{
    public static class PersonalAccessTokenTests
    {
        [Fact]
        public static void InvalidToken_ArgumentException()
        {
            var ex = FluentActions.Invoking(() => (PersonalAccessToken)"asdf")
                .Should()
                .Throw<ArgumentException>();
            
            ex.Which
                .ParamName
                .Should()
                .Be("token");

            ex.Which
                .Message
                .Should()
                .Contain("expected to be null or empty");
        }

        [Fact]
        public static void FromNullToStringToToken() =>
            ((PersonalAccessToken) (string) null)
            .Should()
            .Be(PersonalAccessToken.Empty);

        [Fact]
        public static void FromNullToTokenToString() =>
            ((string) (PersonalAccessToken) null)
            .Should()
            .Be(string.Empty);

        [Fact]
        public static void FromStringWithNull() =>
            FluentActions.Invoking(() => PersonalAccessToken.FromString(null))
                .Should()
                .Throw<ArgumentNullException>();
    }
}