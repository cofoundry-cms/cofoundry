using FluentAssertions;
using FluentAssertions.Execution;
using System.Net;
using System.Net.Http;

namespace Cofoundry.Web.Tests.Integration
{
    public static class LoginRedirectAssertions
    {
        /// <summary>
        /// Asserts that the request has been redirected to the login page for the specified user area.
        /// Note that the web client needs to be created with <c>AllowAutoRedirect = false</c>
        /// to capture the redirect e.g. 
        /// <code>_webApplicationFactory.CreateClient(o => o.AllowAutoRedirect = false)</code>
        /// </summary>
        /// <param name="result">
        /// The request response to check.
        /// </param>
        /// <param name="userArea">The user area with the login page that the request should be redirect to.</param>
        public static void AssertLoginRedirect(HttpResponseMessage result, Domain.Tests.Integration.SeedData.TestUserAreaInfo userArea)
        {
            using (new AssertionScope())
            {
                result.StatusCode.Should().Be(HttpStatusCode.Redirect);
                result.Headers.Location.OriginalString.Should().StartWith("http://localhost" + userArea.Definition.LoginPath);
                var returnUrl = WebUtility.UrlEncode(result.RequestMessage.RequestUri.PathAndQuery.ToString());
                result.Headers.Location.OriginalString.Should().EndWith($"ReturnUrl={returnUrl}");
            }
        }
    }
}
