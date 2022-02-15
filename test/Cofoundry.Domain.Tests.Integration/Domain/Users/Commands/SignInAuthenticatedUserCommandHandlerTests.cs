using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class SignInAuthenticatedUserCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "SignInAuthedCHT-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public SignInAuthenticatedUserCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task WhenValid_IsSuccess()
        {
            var uniqueData = UNIQUE_PREFIX + "ValidSuccess";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userSessionService = app.Services.GetRequiredService<IUserSessionService>();

            var userId = await app.TestData.Users().AddAsync(uniqueData);

            var now = DateTime.UtcNow;
            app.Mocks.MockDateTime(now);

            await contentRepository
                .Users()
                .Authentication()
                .SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
                {
                    UserId = userId
                });

            var sessionUserId = await userSessionService.GetUserIdByUserAreaCodeAsync(TestUserArea1.Code);
            var user = await dbContext.Users.AsNoTracking().FilterById(userId).SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                sessionUserId.Should().Be(sessionUserId);
                user.LastSignInDate.Should().Be(now);

                app.Mocks
                    .CountMessagesPublished<UserSignednMessage>(m =>
                    {
                        return m.UserId == userId && m.UserAreaCode == TestUserArea1.Code;
                    })
                    .Should().Be(1);
            }
        }

        [Fact]
        public async Task WhenPasswordChangeRequired_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "PWChangeReq";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var userId = await app.TestData.Users().AddAsync(uniqueData, c =>
            {
                c.RequirePasswordChange = true;
            });

            await contentRepository
                .Awaiting(r => r.Users().Authentication().SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
                {
                    UserId = userId
                }))
                .Should()
                .ThrowAsync<PasswordChangeRequiredException>()
                .WithErrorCode(UserValidationErrors.Authentication.PasswordChangeRequired.ErrorCode);
        }
    }
}