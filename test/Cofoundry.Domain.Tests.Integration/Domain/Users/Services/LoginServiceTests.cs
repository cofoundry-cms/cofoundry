using Cofoundry.Domain.Tests.Shared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Services
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class LoginServiceTests
    {
        const string UNIQUE_PREFIX = "LoginSvc";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public LoginServiceTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task LogAuthenticatedUserInAsync_SendsMessage()
        {
            using var app = _appFactory.Create();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var user = app.SeededEntities.AdminUser;

            await loginService.LogAuthenticatedUserInAsync(CofoundryAdminUserArea.AreaCode, user.UserId, true);

            app.Mocks
                .CountMessagesPublished<UserLoggedInMessage>(m =>
                {
                    return m.UserId == user.UserId && m.UserAreaCode == CofoundryAdminUserArea.AreaCode;
                })
                .Should().Be(1);
        }

        [Fact]
        public async Task SignOutAsync_WhenLoggedIn_SendsMessage()
        {
            using var app = _appFactory.Create();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var user = app.SeededEntities.AdminUser;

            await loginService.LogAuthenticatedUserInAsync(CofoundryAdminUserArea.AreaCode, user.UserId, true);
            await loginService.SignOutAsync(CofoundryAdminUserArea.AreaCode);

            app.Mocks
                .CountMessagesPublished<UserLoggedOutMessage>(m =>
                {
                    return m.UserId == user.UserId && m.UserAreaCode == CofoundryAdminUserArea.AreaCode;
                })
                .Should().Be(1);
        }

        [Fact]
        public async Task SignOutAsync_WhenNotLoggedIn_DoesNotSendMessage()
        {
            using var app = _appFactory.Create();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var user = app.SeededEntities.AdminUser;

            await loginService.SignOutAsync(CofoundryAdminUserArea.AreaCode);

            app.Mocks
                .CountMessagesPublished<UserLoggedOutMessage>()
                .Should().Be(0);
        }

        [Fact]
        public async Task SignOutAllUserAreasAsync_WhenLoggedIn_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SOA_LogIn";

            using var app = _appFactory.Create();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var cofoundryUser = app.SeededEntities.AdminUser;
            var testUserArea1UserId = await app.TestData.Users().AddAsync(uniqueData);
            await loginService.LogAuthenticatedUserInAsync(CofoundryAdminUserArea.AreaCode, cofoundryUser.UserId, true);
            await loginService.LogAuthenticatedUserInAsync(TestUserArea1.Code, testUserArea1UserId, true);
            await loginService.SignOutAllUserAreasAsync();

            app.Mocks
                .CountMessagesPublished<UserLoggedOutMessage>(m =>
                {
                    return m.UserId == cofoundryUser.UserId && m.UserAreaCode == CofoundryAdminUserArea.AreaCode;
                })
                .Should().Be(1);
            app.Mocks
                .CountMessagesPublished<UserLoggedOutMessage>(m =>
                {
                    return m.UserId == testUserArea1UserId && m.UserAreaCode == TestUserArea1.Code;
                })
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserLoggedOutMessage>()
                .Should().Be(2);
        }
    }
}
