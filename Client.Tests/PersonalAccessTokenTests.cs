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
                .Contain("expected to be empty");
        }

        [Fact]
        public static void FromNullToStringToToken() =>
            ((PersonalAccessToken) null)
            .Should()
            .Be(PersonalAccessToken.Empty);

        [Fact]
        public static void FromNullToTokenToString() =>
            ((string) (PersonalAccessToken) null)
            .Should()
            .Be(string.Empty);

        [Fact]
        public static void FromStringNull() =>
            PersonalAccessToken.FromString(null)
                .Should()
                .Be(PersonalAccessToken.Empty);
        
        [Fact]
        public static void NotEquals() =>
            PersonalAccessToken.Empty
                .Should()
                .NotBe(4);
        
        [Fact]
        public static void DefaultStructShouldBeEmpty()
        {
            PersonalAccessToken token;
            token.Should()
                .Be(PersonalAccessToken.Empty);
        }
        
        [Fact]
        public static void OrdinalEquals()
        {
            var token = new string('x', 52);
            ((PersonalAccessToken) token)
                .Should()
                .NotBe((PersonalAccessToken)token.ToUpper());
        }
        
        [Fact]
        public static void OrdinalHashCode()
        {
            var token = new string('x', 52);
            ((PersonalAccessToken) token).GetHashCode()
                .Should()
                .NotBe(((PersonalAccessToken) token.ToUpper()).GetHashCode());
        }
        
        [Fact]
        public static void OperatorEquals()
        {
            var source = new string('x', 52);
            PersonalAccessToken token1 = source;
            PersonalAccessToken token2 = source;

            (token1 == token2).Should().BeTrue();
        }
        
        [Fact]
        public static void OperatorNotEquals()
        {
            PersonalAccessToken token1 = new string('x', 52);
            PersonalAccessToken token2 = new string('y', 52);

            (token1 != token2).Should().BeTrue();
        }
    }
}