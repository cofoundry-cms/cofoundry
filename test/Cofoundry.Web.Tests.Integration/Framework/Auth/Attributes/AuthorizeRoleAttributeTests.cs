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
    public class AuthorizeRoleAttributeTests
    {
        private readonly DbDependentTestApplicationFactory _appFactory;
        private readonly TestWebApplicationFactory _webApplicationFactory;

        public AuthorizeRoleAttributeTests(
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
            var result = await client.GetAsync(GetRoute("role"));

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task WhenUserNotLoggedIn_RedirectsToLogin()
        {
            using var app = _webApplicationFactory.CreateApp();
            using var client = _webApplicationFactory.CreateClient(o => o.AllowAutoRedirect = false);

            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
            var result = await client.GetAsync(GetRoute("role"));

            LoginRedirectAssertions.AssertLoginRedirect(result, app.SeededEntities.TestUserArea2);
        }

        [Fact]
        public async Task WhenUserNotAuthorized_403()
        {
            using var app = _webApplicationFactory.CreateApp();
            using var client = _webApplicationFactory.CreateClient();

            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleB.User);
            var result = await client.GetAsync(GetRoute("role"));

            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        private string GetRoute(string path)
        {
            return $"/{AuthAttributeTestController.RouteBase}/{path}";
        }
    }
}
