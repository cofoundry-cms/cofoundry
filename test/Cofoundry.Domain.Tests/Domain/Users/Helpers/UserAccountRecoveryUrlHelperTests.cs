using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using Xunit;

namespace Cofoundry.Domain.Tests.Users.Helpers
{
    public class UserAccountRecoveryUrlHelperTests
    {
        private const string SIMPLE_TOKEN = "8cbe40a37b274e02af7c6905f2463c31-1poewiwen4n4";
        private static string ABSOLUTE_BASE_URI = "https://www.cofoundry.com/auth/forgot-password";

        [Theory]
        [InlineData("/auth/forgot-password", "/auth/forgot-password?t=" + SIMPLE_TOKEN)]
        [InlineData("/login/forgot-password/", "/login/forgot-password?t=" + SIMPLE_TOKEN)]
        public void MakeUrl_WhenUrlRelative_ReturnsCorrectUrl(string url, string expected)
        {
            var userAccountRecoveryUrlHelper = new UserAccountRecoveryUrlHelper();
            var result = userAccountRecoveryUrlHelper.MakeUrl(url, SIMPLE_TOKEN);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void MakeUrl_WhenUrlAbsolute_ReturnsCorrectUrl()
        {
            var expected = $"{ ABSOLUTE_BASE_URI }?t={ SIMPLE_TOKEN }";

            var userAccountRecoveryUrlHelper = new UserAccountRecoveryUrlHelper();
            var result = userAccountRecoveryUrlHelper.MakeUrl(ABSOLUTE_BASE_URI, SIMPLE_TOKEN);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void MakeUrl_WhenTokenContainsInvalidChars_EscapesToken()
        {
            var userAccountRecoveryUrlHelper = new UserAccountRecoveryUrlHelper();
            var token = "notEscaped /?:=&\" <>#%{}|\\^[]`";
            var escaped = "notEscaped%20%2F%3F%3A%3D%26%22%20%3C%3E%23%25%7B%7D%7C%5C%5E%5B%5D%60";
            var expected = "/auth/forgot-password?t=" + escaped;

            var result = userAccountRecoveryUrlHelper.MakeUrl("/auth/forgot-password/", token);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ParseFromQuery_EmptyQuery_ReturnsNull()
        {
            var userAccountRecoveryUrlHelper = new UserAccountRecoveryUrlHelper();

            var queryCollection = new QueryCollection();

            var result = userAccountRecoveryUrlHelper.ParseTokenFromQuery(queryCollection);

            Assert.Null(result);
        }

        [Fact]
        public void ParseFromQuery_WhenSimpleQuery_CanParseToken()
        {
            var userAccountRecoveryUrlHelper = new UserAccountRecoveryUrlHelper();

            var url = userAccountRecoveryUrlHelper.MakeUrl(ABSOLUTE_BASE_URI, SIMPLE_TOKEN);
            var query = new Uri(url).Query;
            var values = QueryHelpers.ParseQuery(query);
            var queryCollection = new QueryCollection(values);

            var parsed = userAccountRecoveryUrlHelper.ParseTokenFromQuery(queryCollection);

            Assert.Equal(SIMPLE_TOKEN, parsed);
        }

        [Fact]
        public void ParseFromQuery_WhenTokenContainsInvalidChars_CanParseToken()
        {
            var userAccountRecoveryUrlHelper = new UserAccountRecoveryUrlHelper();

            var token = "notEscaped /?:=&\" <>#%{}|\\^[]`";
            var url = userAccountRecoveryUrlHelper.MakeUrl(ABSOLUTE_BASE_URI, token);
            var query = new Uri(url).Query;
            var values = QueryHelpers.ParseQuery(query);
            var queryCollection = new QueryCollection(values);

            var parsed = userAccountRecoveryUrlHelper.ParseTokenFromQuery(queryCollection);

            Assert.Equal(token, parsed);
        }
    }
}
