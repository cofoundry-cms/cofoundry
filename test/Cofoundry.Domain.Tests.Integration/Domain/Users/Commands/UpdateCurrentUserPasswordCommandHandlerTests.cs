using Cofoundry.Core;
using Cofoundry.Core.Web;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class UpdateCurrentUserPasswordCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "UpdCurUsrPwCHT";
        const string OLD_PASSWORD = "Gr!sh3nk0!";
        const string NEW_PASSWORD = "S3v3rn@ya!";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public UpdateCurrentUserPasswordCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task WhenValid_ChangesPassword()
        {
            var uniqueData = UNIQUE_PREFIX + "Valid_ChangesPw";

            var now = DateTime.UtcNow.AddMinutes(10);
            var updateDate = DateTimeHelper.TruncateMilliseconds(now);

            User originalUserState;
            using (var app = _appFactory.Create())
            {
                var userId = await app.TestData.Users().AddAsync(uniqueData, c =>
                {
                    c.Password = OLD_PASSWORD;
                    c.RequirePasswordChange = true;
                });

                var dbContext = app.Services.GetService<CofoundryDbContext>();
                var contentRepository = app.Services.GetContentRepository();
                var userSessionService = app.Services.GetRequiredService<IUserSessionService>();

                originalUserState = await dbContext
                    .Users
                    .AsNoTracking()
                    .FilterById(userId)
                    .SingleOrDefaultAsync();

                app.Mocks.MockDateTime(updateDate);

                await userSessionService.SignInAsync(TestUserArea1.Code, userId, false);

                await contentRepository.ExecuteCommandAsync(new UpdateCurrentUserPasswordCommand()
                {
                    NewPassword = NEW_PASSWORD,
                    OldPassword = OLD_PASSWORD
                });
            }

            UserCredentialsAuthenticationResult authResult;
            User user = null;

            using (var app = _appFactory.Create())
            {
                var repository = app.Services.GetService<IDomainRepository>();
                var dbContext = app.Services.GetService<CofoundryDbContext>();

                // Use the auth query to verify the password has been changed
                authResult = await repository.ExecuteQueryAsync(new AuthenticateUserCredentialsQuery()
                {
                    UserAreaCode = TestUserArea1.Code,
                    Username = originalUserState.Username,
                    Password = NEW_PASSWORD
                });

                if (authResult?.User != null)
                {
                    user = await dbContext
                        .Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(u => u.UserId == authResult.User.UserId);
                }
            }

            using (new AssertionScope())
            {
                authResult.Should().NotBeNull();
                authResult.IsSuccess.Should().BeTrue();
                user.RequirePasswordChange.Should().BeFalse();
                user.AccountVerifiedDate.Should().BeNull();
                user.LastPasswordChangeDate.Should().Be(updateDate);
                user.SecurityStamp.Should().NotBeNull().And.NotBe(originalUserState.SecurityStamp);
            }
        }

        [Fact]
        public async Task WhenInvalidOldPassword_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "InvOldPw_T";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();

            var userId = await app.TestData.Users().AddAsync(uniqueData, c => c.Password = OLD_PASSWORD );

            await contentRepository
                .Users()
                .Authentication()
                .SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
                {
                    UserId = userId
                });

            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = OLD_PASSWORD,
                OldPassword = NEW_PASSWORD
            };

            await contentRepository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<InvalidCredentialsAuthenticationException>()
                .WithMemberNames(nameof(command.OldPassword));
        }

        [Fact]
        public async Task WhenNotSignedIn_Throws()
        {
            using var app = _appFactory.Create();
            var repository = app.Services.GetService<IDomainRepository>();

            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = NEW_PASSWORD,
                OldPassword = OLD_PASSWORD
            };

            await repository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<PermissionValidationFailedException>();
        }

        [Fact]
        public async Task WhenSystemUser_Throws()
        {
            using var app = _appFactory.Create();
            var repository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = NEW_PASSWORD,
                OldPassword = OLD_PASSWORD
            };

            await repository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<EntityNotFoundException<User>>();
        }

        [Fact]
        public async Task SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SendsMessage";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();

            var userId = await app.TestData.Users().AddAsync(uniqueData, c => c.Password = OLD_PASSWORD);

            await contentRepository
                .Users()
                .Authentication()
                .SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
                {
                    UserId = userId
                });

            await contentRepository
                .Users()
                .Current()
                .UpdatePasswordAsync(new UpdateCurrentUserPasswordCommand()
                {
                    NewPassword = NEW_PASSWORD,
                    OldPassword = OLD_PASSWORD
                });

            using (new AssertionScope())
            {
                app.Mocks
                    .CountMessagesPublished<UserPasswordUpdatedMessage>(m => m.UserId == userId && m.UserAreaCode == TestUserArea1.Code)
                    .Should().Be(1);

                app.Mocks
                    .CountMessagesPublished<UserSecurityStampUpdatedMessage>(m => m.UserId == userId && m.UserAreaCode == TestUserArea1.Code)
                    .Should().Be(1);
            }
        }

        [Fact]
        public async Task SendsMail()
        {
            var uniqueData = UNIQUE_PREFIX + "SendsMail";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();

            var userId = await app.TestData.Users().AddAsync(uniqueData, c => c.Password = OLD_PASSWORD);

            await contentRepository
                .Users()
                .Authentication()
                .SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
                {
                    UserId = userId
                });

            await contentRepository
                .Users()
                .Current()
                .UpdatePasswordAsync(new UpdateCurrentUserPasswordCommand()
                {
                    NewPassword = NEW_PASSWORD,
                    OldPassword = OLD_PASSWORD
                });

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var siteUrlResolver = app.Services.GetRequiredService<ISiteUrlResolver>();
            var signInUrl = siteUrlResolver.MakeAbsolute(app.SeededEntities.TestUserArea1.Definition.SignInPath);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleOrDefaultAsync();

            app.Mocks
                .CountDispatchedMail(
                    user.Email,
                    "password",
                    "Test Site",
                    "has been changed",
                    "username for this account is " + user.Username,
                    signInUrl
                )
                .Should().Be(1);
        }
    }
}