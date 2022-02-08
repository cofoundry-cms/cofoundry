using Cofoundry.Domain.Tests.Integration;
using Cofoundry.Domain.Tests.Shared.Assertions;
using Cofoundry.Web.Tests.Integration.TestWebApp;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Web.Tests.Integration.Framework.Auth.Attributes
{
    [Collection(nameof(DbDependentTestApplicationFactory))]
    public class AuthorizeUserAreaAttributeTests
    {
        private readonly DbDependentTestApplicationFactory _appFactory;
        private readonly TestWebApplicationFactory _webApplicationFactory;

        public AuthorizeUserAreaAttributeTests(
            DbDependentTestApplicationFactory appFactory,
            TestWebApplicationFactory webApplicationFactory
            )
        {
            _appFactory = appFactory;
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task WhenUserValid_OK()
        {
            using var app = _webApplicationFactory.CreateApp();
            using var client = _webApplicationFactory.CreateClient();

            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);
            var result = await client.GetAsync(GetRoute("user-area"));

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task WhenUserNotSignedIn_RedirectsToSignIn()
        {
            using var app = _webApplicationFactory.CreateApp();
            using var client = _webApplicationFactory.CreateClient(o => o.AllowAutoRedirect = false);

            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
            var result = await client.GetAsync(GetRoute("user-area"));

            SignInRedirectAssertions.AssertSignInRedirect(result, app.SeededEntities.TestUserArea2);
        }

        private string GetRoute(string path)
        {
            return $"/{AuthAttributeTestController.RouteBase}/{path}";
        }
    }
}
