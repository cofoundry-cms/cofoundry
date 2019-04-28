using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class PasswordResetUrlHelperTests
    {
        private const string SIMPLE_TOKEN = "1poewiwen4n4";
        private const string ID_STRING = "8cbe40a37b274e02af7c6905f2463c31";
        private static Guid ID = Guid.Parse(ID_STRING);
        private static Uri ABSOLUTE_BASE_URI = new Uri("https://www.cofoundry.com/auth/forgot-password");

        #region MakeUrl

        [Theory]
        [InlineData("/auth/forgot-password", "/auth/forgot-password?i=" + ID_STRING + "&t=" + SIMPLE_TOKEN)]
        [InlineData("/login/forgot-password/", "/login/forgot-password?i=" + ID_STRING + "&t=" + SIMPLE_TOKEN)]
        public void MakeUrl_WhenUrlRelative_ReturnsCorrectUrl(string url, string expected)
        {
            var passwordResetUrlHelper = new PasswordResetUrlHelper();
            var baseUri = new Uri(url, UriKind.Relative);
            var result = passwordResetUrlHelper.MakeUrl(baseUri, ID, SIMPLE_TOKEN);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void MakeUrl_WhenUrlAbsolute_ReturnsCorrectUrl()
        {
            var expected = $"{ ABSOLUTE_BASE_URI }?i={ ID_STRING }&t={ SIMPLE_TOKEN }";

            var passwordResetUrlHelper = new PasswordResetUrlHelper();
            var result = passwordResetUrlHelper.MakeUrl(ABSOLUTE_BASE_URI, ID, SIMPLE_TOKEN);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void MakeUrl_WhenTokenContainsInvalidChars_EscapesToken()
        {
            var passwordResetUrlHelper = new PasswordResetUrlHelper();
            var token = "notEscaped /?:=&\" <>#%{}|\\^[]`";
            var escaped = "notEscaped%20%2F%3F%3A%3D%26%22%20%3C%3E%23%25%7B%7D%7C%5C%5E%5B%5D%60";
            var expected = "/auth/forgot-password?i=" + ID_STRING + "&t=" + escaped;

            var baseUri = new Uri("/auth/forgot-password/", UriKind.Relative);
            var result = passwordResetUrlHelper.MakeUrl(baseUri, ID, token);

            Assert.Equal(expected, result);
        }

        #endregion

        #region ParseFromQuery

        [Fact]
        public void ParseFromQuery_EmptyQuery_ReturnsEmpty()
        {
            var passwordResetUrlHelper = new PasswordResetUrlHelper();

            var queryCollection = new QueryCollection();

            var result = passwordResetUrlHelper.ParseFromQuery(queryCollection);

            Assert.NotNull(result);
            Assert.Equal(Guid.Empty, result.UserPasswordResetRequestId);
            Assert.Null(result.Token);
        }

        [Fact]
        public void ParseFromQuery_WhenSimpleQuery_CanParseUserPasswordResetRequestId()
        {
            var passwordResetUrlHelper = new PasswordResetUrlHelper();

            var url = passwordResetUrlHelper.MakeUrl(ABSOLUTE_BASE_URI, ID, SIMPLE_TOKEN);
            var query = new Uri(url).Query;
            var values = QueryHelpers.ParseQuery(query);
            var queryCollection = new QueryCollection(values);

            var parsed = passwordResetUrlHelper.ParseFromQuery(queryCollection);
            Assert.Equal(ID, parsed.UserPasswordResetRequestId);
        }

        [Fact]
        public void ParseFromQuery_WhenSimpleQuery_CanParseToken()
        {
            var passwordResetUrlHelper = new PasswordResetUrlHelper();

            var url = passwordResetUrlHelper.MakeUrl(ABSOLUTE_BASE_URI, ID, SIMPLE_TOKEN);
            var query = new Uri(url).Query;
            var values = QueryHelpers.ParseQuery(query);
            var queryCollection = new QueryCollection(values);

            var parsed = passwordResetUrlHelper.ParseFromQuery(queryCollection);

            Assert.Equal(SIMPLE_TOKEN, parsed.Token);
        }

        [Fact]
        public void ParseFromQuery_WhenTokenContainsInvalidChars_CanParseToken()
        {
            var passwordResetUrlHelper = new PasswordResetUrlHelper();

            var token = "notEscaped /?:=&\" <>#%{}|\\^[]`";
            var url = passwordResetUrlHelper.MakeUrl(ABSOLUTE_BASE_URI, ID, token);
            var query = new Uri(url).Query;
            var values = QueryHelpers.ParseQuery(query);
            var queryCollection = new QueryCollection(values);

            var parsed = passwordResetUrlHelper.ParseFromQuery(queryCollection);

            Assert.Equal(token, parsed.Token);
        }

        #endregion
    }
}
