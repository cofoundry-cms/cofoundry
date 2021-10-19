using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.Tests.Integration;
using Cofoundry.Domain.Tests.Shared.Assertions;
using Cofoundry.Domain.Tests.Shared.Mocks;
using Cofoundry.Web.Tests.Integration.TestWebApp;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
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
            using var app = _webApplicationFactory.CreateApp();

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
            using var app = _webApplicationFactory.CreateApp();

            var page = app.SeededEntities.TestDirectory.GenericPage;
            var result = await client.GetAsync(page.FullPath);
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{page.Title}</h1>*");
        }

        [Fact]
        public async Task CanRenderCustomEntityPage()
        {
            using var client = _webApplicationFactory.CreateClient();
            using var app = _webApplicationFactory.CreateApp();

            var page = app.SeededEntities.TestDirectory.CustomEntityPage;
            var customEntity = app.SeededEntities.TestCustomEntity;
            var path = page.GetFullPath(customEntity.CustomEntityId);
            var result = await client.GetAsync(path);
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{page.Title}</h1>*");
            content.Should().Match($"*<h2>{customEntity.Title}</h2>*");
        }

        [Fact]
        public async Task WhenPageAccessRuleForUserAreaAndUserValid_OK()
        {
            var uniqueData = UNIQUE_PREFIX + "PAR4UserAreaAndUserValid";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(pageId, app.SeededEntities.TestUserArea1.UserAreaCode);

            using var client = _webApplicationFactory.CreateClient();
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.User);
            var clientCache = _webApplicationFactory.Services.GetRequiredService<IPageCache>();

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }
        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task WhenPageAccessRuleForUserAreaAndUserInvalid_ReturnsCorrectAction(RouteAccessRuleViolationAction routeAccessRuleViolationAction)
        {
            var uniqueData = UNIQUE_PREFIX + "PAR4UserAreaAndUserInv" + routeAccessRuleViolationAction.ToString().Substring(0, 2);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(pageId, app.SeededEntities.TestUserArea1.UserAreaCode, c => c.ViolationAction = routeAccessRuleViolationAction);

            using var client = _webApplicationFactory
                .WithServices(s => s.TurnOnDeveloperExceptionPage())
                .CreateClient(new WebApplicationFactoryClientOptions()
                {
                    AllowAutoRedirect = false
                });
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.User);

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, routeAccessRuleViolationAction, app.SeededEntities.TestUserArea1);
        }

        [Fact]
        public async Task WhenPageAccessRuleForRoleAndUserValid_OK()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenPAR4RoleAndUserValid";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(pageId, app.SeededEntities.TestUserArea1.UserAreaCode, c => c.RoleId = app.SeededEntities.TestUserArea1.RoleId);

            using var client = _webApplicationFactory.CreateClient();
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.User);

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }

        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task WhenPageAccessRuleForRoleAndUserInvalid_ReturnsCorrectAction(RouteAccessRuleViolationAction routeAccessRuleViolationAction)
        {
            var uniqueData = UNIQUE_PREFIX + "PAR4RoleAndUserInv" + routeAccessRuleViolationAction.ToString().Substring(0, 2);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(pageId, app.SeededEntities.TestUserArea1.UserAreaCode, c =>
            {
                c.ViolationAction = routeAccessRuleViolationAction;
                c.RoleId = app.SeededEntities.TestUserArea1.RoleId;
            });

            using var client = _webApplicationFactory
                .WithServices(s => s.TurnOnDeveloperExceptionPage())
                .CreateClient(new WebApplicationFactoryClientOptions()
                {
                    AllowAutoRedirect = false
                });
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.User);

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, routeAccessRuleViolationAction, app.SeededEntities.TestUserArea1);
        }

        [Fact]
        public async Task WhenDirectoryAccessRuleForUserAreaAndUserValid_OK()
        {
            var uniqueData = UNIQUE_PREFIX + "DAR4UserAreaAndUserValid";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(directoryId, app.SeededEntities.TestUserArea1.UserAreaCode);

            using var client = _webApplicationFactory.CreateClient();
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.User);
            var clientCache = _webApplicationFactory.Services.GetRequiredService<IPageCache>();

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }

        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task WhenDirectoryAccessRuleForUserAreaAndUserInvalid_ReturnsCorrectAction(RouteAccessRuleViolationAction routeAccessRuleViolationAction)
        {
            var uniqueData = UNIQUE_PREFIX + "DAR4UserAreaAndUserInv" + routeAccessRuleViolationAction.ToString().Substring(0, 2);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(directoryId, app.SeededEntities.TestUserArea1.UserAreaCode, c => c.ViolationAction = routeAccessRuleViolationAction);

            using var client = _webApplicationFactory
                .WithServices(s => s.TurnOnDeveloperExceptionPage())
                .CreateClient(new WebApplicationFactoryClientOptions()
                {
                    AllowAutoRedirect = false
                });
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.User);

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, routeAccessRuleViolationAction, app.SeededEntities.TestUserArea1);

        }

        [Fact]
        public async Task WhenDirectoryAccessRuleForRoleAndUserValid_OK()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenDAR4RoleAndUserValid";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(directoryId, app.SeededEntities.TestUserArea1.UserAreaCode, c => c.RoleId = app.SeededEntities.TestUserArea1.RoleId);

            using var client = _webApplicationFactory.CreateClient();
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.User);

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }

        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task WhenDirectoryAccessRuleForRoleAndUserInvalid_ReturnsCorrectAction(RouteAccessRuleViolationAction routeAccessRuleViolationAction)
        {
            var uniqueData = UNIQUE_PREFIX + "DAR4RoleAndUserInv" + routeAccessRuleViolationAction.ToString().Substring(0, 2);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(directoryId, app.SeededEntities.TestUserArea1.UserAreaCode, c =>
            {
                c.ViolationAction = routeAccessRuleViolationAction;
                c.RoleId = app.SeededEntities.TestUserArea1.RoleId;
            });

            using var client = _webApplicationFactory
                .WithServices(s => s.TurnOnDeveloperExceptionPage())
                .CreateClient(new WebApplicationFactoryClientOptions()
                {
                    AllowAutoRedirect = false
                });
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.User);

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, routeAccessRuleViolationAction, app.SeededEntities.TestUserArea1);
        }

        private async Task AssertAccessRuleResponseAsync(
            HttpResponseMessage result,
            RouteAccessRuleViolationAction routeAccessRuleViolationAction,
            Domain.Tests.Integration.SeedData.TestUserAreaInfo userArea
            )
        {
            switch (routeAccessRuleViolationAction)
            {
                case RouteAccessRuleViolationAction.Error:
                    await result.Should().BeDeveloperPageExceptionAsync<AccessRuleViolationException>();
                    break;
                case RouteAccessRuleViolationAction.NotFound:
                    result.StatusCode.Should().Be(HttpStatusCode.NotFound);
                    break;
                case RouteAccessRuleViolationAction.RedirectToLogin:
                    using (new AssertionScope())
                    {
                        result.StatusCode.Should().Be(HttpStatusCode.Redirect);
                        result.Headers.Location.OriginalString.Should().StartWith(userArea.Definition.LoginPath);
                        var returnUrl = WebUtility.UrlEncode(result.RequestMessage.RequestUri.PathAndQuery.ToString());
                        result.Headers.Location.OriginalString.Should().EndWith($"ReturnUrl={returnUrl}");
                    }
                    break;
                default:
                    throw new NotImplementedException($"Known {nameof(RouteAccessRuleViolationAction)} '{routeAccessRuleViolationAction}'");
            }
        }
    }
}
