using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class SignOutCurrentUserFromAllUserAreasCommand
    {
        const string UNIQUE_PREFIX = "SOCurUsrAllUACHT-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public SignOutCurrentUserFromAllUserAreasCommand(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanSOMultiple()
        {
            var uniqueData = UNIQUE_PREFIX + "SOMulti";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
            var userArea2 = app.SeededEntities.TestUserArea2;

            var user1Id = await app.TestData.Users().AddAsync(uniqueData + "1");
            var user2Id = await app.TestData.Users().AddAsync(uniqueData + "2", c =>
            {
                c.UserAreaCode = userArea2.UserAreaCode;
                c.RoleId = userArea2.RoleA.RoleId;
            });

            await userSessionService.SignInAsync(TestUserArea1.Code, user1Id, true);
            await userSessionService.SignInAsync(userArea2.UserAreaCode, user2Id, true);

            var userId1BeforeSignOut = await userSessionService.GetUserIdByUserAreaCodeAsync(TestUserArea1.Code);
            var userId2BeforeSignOut = await userSessionService.GetUserIdByUserAreaCodeAsync(userArea2.UserAreaCode);

            await contentRepository
                .Users()
                .Authentication()
                .SignOutAllUserAreasAsync();

            var userId1AfterSignOut = await userSessionService.GetUserIdByUserAreaCodeAsync(TestUserArea1.Code);
            var userId2AfterSignOut = await userSessionService.GetUserIdByUserAreaCodeAsync(userArea2.UserAreaCode);

            using (new AssertionScope())
            {
                userId1BeforeSignOut.Should().Be(user1Id);
                userId2BeforeSignOut.Should().Be(user2Id);
                userId1AfterSignOut.Should().BeNull();
                userId2AfterSignOut.Should().BeNull();
            }
        }

        [Fact]
        public async Task SendsMessages()
        {
            var uniqueData = UNIQUE_PREFIX + "Messages";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
            var userArea2 = app.SeededEntities.TestUserArea2;

            var user1Id = await app.TestData.Users().AddAsync(uniqueData + "1");
            var user2Id = await app.TestData.Users().AddAsync(uniqueData + "2", c =>
            {
                c.UserAreaCode = userArea2.UserAreaCode;
                c.RoleId = userArea2.RoleA.RoleId;
            });

            await userSessionService.SignInAsync(TestUserArea1.Code, user1Id, true);
            await userSessionService.SignInAsync(userArea2.UserAreaCode, user2Id, true);

            await contentRepository
                .Users()
                .Authentication()
                .SignOutAllUserAreasAsync();

            using (new AssertionScope())
            {
                app.Mocks
                    .CountMessagesPublished<UserSignedOutMessage>(m =>
                    {
                        return m.UserId == user1Id && m.UserAreaCode == TestUserArea1.Code;
                    })
                    .Should().Be(1);
                app.Mocks
                    .CountMessagesPublished<UserSignedOutMessage>(m =>
                    {
                        return m.UserId == user2Id && m.UserAreaCode == TestUserArea2.Code;
                    })
                    .Should().Be(1);

                app.Mocks
                    .CountMessagesPublished<UserSignedOutMessage>()
                    .Should().Be(2);
            }
        }
    }
}