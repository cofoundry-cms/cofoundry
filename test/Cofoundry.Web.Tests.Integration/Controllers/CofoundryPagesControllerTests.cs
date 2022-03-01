using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.Tests.Integration;
using Cofoundry.Domain.Tests.Shared.Assertions;
using Cofoundry.Domain.Tests.Shared.Mocks;
using Cofoundry.Web.Tests.Integration.TestWebApp;
using FluentAssertions;
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
            content.Should().Match("*an error has occurred*");
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
            var result = await client.GetAsync(page.FullUrlPath);
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
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }

        [Theory]
        [InlineData(AccessRuleViolationAction.Error)]
        [InlineData(AccessRuleViolationAction.NotFound)]
        public async Task WhenPageAccessRuleForUserAreaAndUserInvalid_ReturnsCorrectAction(AccessRuleViolationAction routeAccessRuleViolationAction)
        {
            var uniqueData = UNIQUE_PREFIX + "PAR4UserAreaAndUserInv" + routeAccessRuleViolationAction.ToString().Substring(0, 2);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(pageId, app.SeededEntities.TestUserArea1.UserAreaCode, null, c => c.ViolationAction = routeAccessRuleViolationAction);

            using var client = _webApplicationFactory.CreateClientWithServices(s => s.TurnOnDeveloperExceptionPage());
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);
            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, routeAccessRuleViolationAction);
        }

        [Fact]
        public async Task WhenPageAccessRuleForRoleAndUserValid_OK()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenPAR4RoleAndUserValid";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(pageId, app.SeededEntities.TestUserArea1.UserAreaCode, app.SeededEntities.TestUserArea1.RoleA.RoleId);

            using var client = _webApplicationFactory.CreateClient();
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }

        [Theory]
        [InlineData(AccessRuleViolationAction.Error)]
        [InlineData(AccessRuleViolationAction.NotFound)]
        public async Task WhenPageAccessRuleForRoleAndUserInvalid_ReturnsCorrectAction(AccessRuleViolationAction routeAccessRuleViolationAction)
        {
            var uniqueData = UNIQUE_PREFIX + "PAR4RoleAndUserInv" + routeAccessRuleViolationAction.ToString().Substring(0, 2);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(
                pageId,
                app.SeededEntities.TestUserArea1.UserAreaCode,
                app.SeededEntities.TestUserArea1.RoleA.RoleId,
                c => c.ViolationAction = routeAccessRuleViolationAction
                );

            using var client = _webApplicationFactory.CreateClientWithServices(s => s.TurnOnDeveloperExceptionPage());
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);
            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, routeAccessRuleViolationAction);
        }

        [Fact]
        public async Task WhenPageAccessRuleWithRedirectAndNotSignedIn_RedirectsToSignIn()
        {
            var uniqueData = UNIQUE_PREFIX + "PARWithRedirectNotSignedIn";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(
                pageId,
                app.SeededEntities.TestUserArea1.UserAreaCode,
                null,
                c => c.UserAreaCodeForSignInRedirect = app.SeededEntities.TestUserArea1.UserAreaCode
                );

            using var client = _webApplicationFactory.CreateClient(o => o.AllowAutoRedirect = false);
            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            SignInRedirectAssertions.AssertSignInRedirect(result, app.SeededEntities.TestUserArea1);
        }

        [Fact]
        public async Task WhenPageAccessRuleWithRedirectAndInvalidCredentials_ReturnsDefaultAction()
        {
            var uniqueData = UNIQUE_PREFIX + "PARWithRedirectInvalidCred";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");
            var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "2", directory1Id);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directory2Id, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(pageId, app.SeededEntities.TestUserArea1.UserAreaCode);

            using var client = _webApplicationFactory.CreateClientWithServices(s => s.TurnOnDeveloperExceptionPage());
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);
            var result = await client.GetAsync($"/{sluggedUniqueData}1/{sluggedUniqueData}2/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, AccessRuleViolationAction.Error);
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
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
            var clientCache = _webApplicationFactory.Services.GetRequiredService<IPageCache>();

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }

        [Theory]
        [InlineData(AccessRuleViolationAction.Error)]
        [InlineData(AccessRuleViolationAction.NotFound)]
        public async Task WhenDirectoryAccessRuleForUserAreaAndUserInvalid_ReturnsCorrectAction(AccessRuleViolationAction routeAccessRuleViolationAction)
        {
            var uniqueData = UNIQUE_PREFIX + "DAR4UserAreaAndUserInv" + routeAccessRuleViolationAction.ToString().Substring(0, 2);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(
                directoryId,
                app.SeededEntities.TestUserArea1.UserAreaCode,
                null,
                c => c.ViolationAction = routeAccessRuleViolationAction
                );

            using var client = _webApplicationFactory.CreateClientWithServices(s => s.TurnOnDeveloperExceptionPage());
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);
            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, routeAccessRuleViolationAction);
        }

        [Fact]
        public async Task WhenDirectoryAccessRuleForRoleAndUserValid_OK()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenDAR4RoleAndUserValid";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(directoryId, app.SeededEntities.TestUserArea1.UserAreaCode, app.SeededEntities.TestUserArea1.RoleA.RoleId);

            using var client = _webApplicationFactory.CreateClient();
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }

        [Theory]
        [InlineData(AccessRuleViolationAction.Error)]
        [InlineData(AccessRuleViolationAction.NotFound)]
        public async Task WhenDirectoryAccessRuleForRoleAndUserInvalid_ReturnsCorrectAction(AccessRuleViolationAction routeAccessRuleViolationAction)
        {
            var uniqueData = UNIQUE_PREFIX + "DAR4RoleAndUserInv" + routeAccessRuleViolationAction.ToString().Substring(0, 2);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(
                directoryId,
                app.SeededEntities.TestUserArea1.UserAreaCode,
                app.SeededEntities.TestUserArea1.RoleA.RoleId,
                c => c.ViolationAction = routeAccessRuleViolationAction
                );

            using var client = _webApplicationFactory.CreateClientWithServices(s => s.TurnOnDeveloperExceptionPage());
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);

            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            await AssertAccessRuleResponseAsync(result, routeAccessRuleViolationAction);
        }

        [Fact]
        public async Task WhenDirectoryAccessRuleWithRedirectAndNotSignedIn_RedirectsToSignIn()
        {
            var uniqueData = UNIQUE_PREFIX + "DARWithRedirectNotSignedIn";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(
                directoryId,
                app.SeededEntities.TestUserArea1.UserAreaCode,
                null,
                c => c.UserAreaCodeForSignInRedirect = app.SeededEntities.TestUserArea1.UserAreaCode
                );

            using var client = _webApplicationFactory.CreateClient(o => o.AllowAutoRedirect = false);
            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");

            SignInRedirectAssertions.AssertSignInRedirect(result, app.SeededEntities.TestUserArea1);
        }

        [Fact]
        public async Task WhenDirectoryAccessRuleWithRedirectAndInvalidCredentials_ReturnsDefaultAction()
        {
            var uniqueData = UNIQUE_PREFIX + "DARWithRedirectInvalidCred";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");
            var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "2", directory1Id);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directory2Id, c => c.Publish = true);
            await app.TestData.PageDirectories().AddAccessRuleAsync(directory1Id, app.SeededEntities.TestUserArea1.UserAreaCode);

            using var client = _webApplicationFactory.CreateClientWithServices(s => s.TurnOnDeveloperExceptionPage());
            var result = await client.GetAsync($"/{sluggedUniqueData}1/{sluggedUniqueData}2/{sluggedUniqueData}");
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);

            await AssertAccessRuleResponseAsync(result, AccessRuleViolationAction.Error);
        }

        [Fact]
        public async Task WhenPageAccessRuleForNonDefaultUserArea_OK()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenPAR4NonDefaultUserArea";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _webApplicationFactory.CreateApp();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddAccessRuleAsync(pageId, app.SeededEntities.TestUserArea2.UserAreaCode);

            using var client = _webApplicationFactory.CreateClient();
            await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);
            var result = await client.GetAsync($"/{sluggedUniqueData}/{sluggedUniqueData}");
            var content = await result.Content.ReadAsStringAsync();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Match($"*<h1>{uniqueData}</h1>*");
        }


        private async Task AssertAccessRuleResponseAsync(
            HttpResponseMessage result,
            AccessRuleViolationAction routeAccessRuleViolationAction
            )
        {
            switch (routeAccessRuleViolationAction)
            {
                case AccessRuleViolationAction.Error:
                    await result.Should().BeDeveloperPageExceptionAsync<AccessRuleViolationException>();
                    break;
                case AccessRuleViolationAction.NotFound:
                    result.StatusCode.Should().Be(HttpStatusCode.NotFound);
                    break;
                default:
                    throw new NotImplementedException($"Known {nameof(AccessRuleViolationAction)} '{routeAccessRuleViolationAction}'");
            }
        }
    }
}
