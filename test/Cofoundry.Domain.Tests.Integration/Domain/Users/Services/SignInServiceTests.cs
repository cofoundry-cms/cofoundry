using Cofoundry.Domain.Tests.Shared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Services
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class SignInServiceTests
    {
        const string UNIQUE_PREFIX = "SignInSvc";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public SignInServiceTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task SignInAuthenticatedUserAsync_SendsMessage()
        {
            using var app = _appFactory.Create();
            var signInService = app.Services.GetRequiredService<IUserSignInService>();
            var user = app.SeededEntities.AdminUser;

            await signInService.SignInAuthenticatedUserAsync(CofoundryAdminUserArea.Code, user.UserId, true);

            app.Mocks
                .CountMessagesPublished<UserSignednMessage>(m =>
                {
                    return m.UserId == user.UserId && m.UserAreaCode == CofoundryAdminUserArea.Code;
                })
                .Should().Be(1);
        }

        [Fact]
        public async Task SignOutAsync_WhenSignedIn_SendsMessage()
        {
            using var app = _appFactory.Create();
            var signInService = app.Services.GetRequiredService<IUserSignInService>();
            var user = app.SeededEntities.AdminUser;

            await signInService.SignInAuthenticatedUserAsync(CofoundryAdminUserArea.Code, user.UserId, true);
            await signInService.SignOutAsync(CofoundryAdminUserArea.Code);

            app.Mocks
                .CountMessagesPublished<UserSignedOutMessage>(m =>
                {
                    return m.UserId == user.UserId && m.UserAreaCode == CofoundryAdminUserArea.Code;
                })
                .Should().Be(1);
        }

        [Fact]
        public async Task SignOutAsync_WhenNotSignedIn_DoesNotSendMessage()
        {
            using var app = _appFactory.Create();
            var signInService = app.Services.GetRequiredService<IUserSignInService>();
            var user = app.SeededEntities.AdminUser;

            await signInService.SignOutAsync(CofoundryAdminUserArea.Code);

            app.Mocks
                .CountMessagesPublished<UserSignedOutMessage>()
                .Should().Be(0);
        }

        [Fact]
        public async Task SignOutAllUserAreasAsync_WhenSignedIn_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SOA_SignedIn";

            using var app = _appFactory.Create();
            var signInService = app.Services.GetRequiredService<IUserSignInService>();
            var cofoundryUser = app.SeededEntities.AdminUser;
            var testUserArea1UserId = await app.TestData.Users().AddAsync(uniqueData);
            await signInService.SignInAuthenticatedUserAsync(CofoundryAdminUserArea.Code, cofoundryUser.UserId, true);
            await signInService.SignInAuthenticatedUserAsync(TestUserArea1.Code, testUserArea1UserId, true);
            await signInService.SignOutAllUserAreasAsync();

            app.Mocks
                .CountMessagesPublished<UserSignedOutMessage>(m =>
                {
                    return m.UserId == cofoundryUser.UserId && m.UserAreaCode == CofoundryAdminUserArea.Code;
                })
                .Should().Be(1);
            app.Mocks
                .CountMessagesPublished<UserSignedOutMessage>(m =>
                {
                    return m.UserId == testUserArea1UserId && m.UserAreaCode == TestUserArea1.Code;
                })
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserSignedOutMessage>()
                .Should().Be(2);
        }
    }
}
