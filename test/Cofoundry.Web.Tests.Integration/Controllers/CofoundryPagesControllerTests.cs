using Cofoundry.Domain;
using Cofoundry.Domain.Tests.Integration;
using Cofoundry.Domain.Tests.Shared.Assertions;
using Cofoundry.Domain.Tests.Shared.Mocks;
using FluentAssertions;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Web.Tests.Integration.Controllers
{
    [Collection(nameof(DbDependentTestApplicationFactory))]
    public class CofoundryPagesControllerTests
    {
        const string UNIQUE_PREFIX = "CFPagesCtrl ";

        private readonly DbDependentTestApplicationFactory _appFactory;
        private readonly TestWebApplicationFactory _webApplicationFactory;

        public CofoundryPagesControllerTests(
            DbDependentTestApplicationFactory appFactory,
            TestWebApplicationFactory webApplicationFactory
            )
        {
            _appFactory = appFactory;
            _webApplicationFactory = webApplicationFactory;
        }

        [Theory]
        [InlineData("/not-a-page")]
        [InlineData("/test-directory/test-page")]
        public async Task WhenSiteNotSetup_Throws(string path)
        {
            using var client = _webApplicationFactory.CreateClientWithServices(s =>
            {
                s.TurnOnDeveloperExceptionPage();
                s.MockHandler<GetSettingsQuery<InternalSettings>, InternalSettings>(new InternalSettings() { IsSetup = false });
            });

            var result = await client.GetAsync(path);
            
            await result.Should().BeDeveloperPageExceptionAsync("*Cofoundry * not * setup*");
        }

        [Fact]
        public async Task WhenError_ReturnsErrorPage()
        {
            using var client = _webApplicationFactory.CreateClientWithServices(s =>
            {
                s.MockHandler<GetSettingsQuery<InternalSettings>, InternalSettings>(new InternalSettings() { IsSetup = false });
            });

            var result = await client.GetAsync("/not-a-page");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            content.Should().Match("*an error has occured*");
        }

        [Fact]
        public async Task WhenPageNotExists_Returns404()
        {
            using var client = _webApplicationFactory.CreateClient();

            var result = await client.GetAsync("/not-a-page");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().Match("*Page not found*");
        }

        [Fact]
        public async Task WhenCustomEntityPageNotExists_Returns404()
        {
            using var client = _webApplicationFactory.CreateClient();
            using var app = _appFactory.Create();

            var page = app.SeededEntities.TestDirectory.CustomEntityPage;
            var result = await client.GetAsync(page.GetFullPath(Int32.MaxValue));
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().Match("*Page not found*");
        }

        [Fact]
        public async Task CanRenderGenericPage()
        {
            using var client = _webApplicationFactory.CreateClient();
            using var app = _appFactory.Create();

            var page = app.SeededEntities.TestDirectory.GenericPage;
            var result = await client.GetAsync(page.FullPath);
            var content = await result.Content.ReadAsStringAsync();

            var title = $"<h1>{page.Title}</h1>";
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CanRenderCustomEntityPage()
        {
            using var client = _webApplicationFactory.CreateClient();
            using var app = _appFactory.Create();

            var page = app.SeededEntities.TestDirectory.CustomEntityPage;
            var customEntity = app.SeededEntities.TestCustomEntity;
            var path = page.GetFullPath(customEntity.CustomEntityId);
            var result = await client.GetAsync(path);
            var content = await result.Content.ReadAsStringAsync();

            var pageTitle = $"<h1>{page.Title}</h1>";
            var customEntityTitle = $"<h2>{customEntity.Title}</h2>";
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task WhenPageAccessRuleForUserAreaAndUserValid_OK()
        {
            throw new NotImplementedException();
        }
        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task WhenPageAccessRuleForUserAreaAndUserInvalid_ReturnsCorrectAction(RouteAccessRuleViolationAction routeAccessRuleViolationAction)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task WhenPageAccessRuleForRoleAndUserValid_OK()
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task WhenPageAccessRuleForRoleAndUserInvalid_ReturnsCorrectAction(RouteAccessRuleViolationAction routeAccessRuleViolationAction)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task WhenDirectoryAccessRuleForUserAreaAndUserValid_OK()
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task WhenDirectoryAccessRuleForUserAreaAndUserValid_ReturnsCorrectAction(RouteAccessRuleViolationAction routeAccessRuleViolationAction)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task WhenDirectoryAccessRuleForRoleAndUserValid_OK()
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task WhenDirectoryAccessRuleForRoleAndUserInvalid_ReturnsCorrectAction(RouteAccessRuleViolationAction routeAccessRuleViolationAction)
        {
            throw new NotImplementedException();
        }
    }
}
