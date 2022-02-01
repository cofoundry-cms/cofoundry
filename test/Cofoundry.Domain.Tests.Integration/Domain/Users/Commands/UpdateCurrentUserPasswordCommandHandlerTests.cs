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
        const string TEST_DOMAIN = "@UpdateCurrentUserPasswordCommandHandlerTests.example.com";
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
            var username = "WhenValid_ChangesPassword" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username);
            var now = DateTime.UtcNow.AddMinutes(10);
            var updateDate = DateTimeHelper.TruncateMilliseconds(now);
            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = NEW_PASSWORD,
                OldPassword = OLD_PASSWORD
            };

            User originalUserState;
            using (var app = _appFactory.Create())
            {
                var dbContext = app.Services.GetService<CofoundryDbContext>();
                originalUserState = await dbContext
                    .Users
                    .AsNoTracking()
                    .FilterById(userId)
                    .SingleOrDefaultAsync();

                app.Mocks.MockDateTime(updateDate);

                var loginService = app.Services.GetService<ILoginService>();
                await loginService.LogAuthenticatedUserInAsync(TestUserArea1.Code, userId, false);

                var repository = app.Services.GetService<IDomainRepository>();
                await repository.ExecuteCommandAsync(command);
            }

            UserCredentialsValidationResult authResult;
            User user = null;

            using (var app = _appFactory.Create())
            {
                var repository = app.Services.GetService<IDomainRepository>();
                var dbContext = app.Services.GetService<CofoundryDbContext>();

                // Use the auth query to verify the password has been changed
                authResult = await repository.ExecuteQueryAsync(new ValidateUserCredentialsQuery()
                {
                    UserAreaCode = TestUserArea1.Code,
                    Username = username,
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
        public async Task WhenInvalidExistingPassword_Throws()
        {
            var username = "WhenInvalidExistingPassword_ThrowsValidationException" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username);

            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = OLD_PASSWORD,
                OldPassword = NEW_PASSWORD
            };

            using var app = _appFactory.Create();

            var repository = app.Services.GetService<IDomainRepository>();
            var loginService = app.Services.GetService<ILoginService>();
            await loginService.LogAuthenticatedUserInAsync(TestUserArea1.Code, userId, false);

            await repository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<InvalidCredentialsAuthenticationException>()
                .WithMemberNames(nameof(command.OldPassword));
        }

        [Fact]
        public async Task WhenNotLoggedIn_Throws()
        {
            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = NEW_PASSWORD,
                OldPassword = OLD_PASSWORD
            };

            using var app = _appFactory.Create();

            var repository = app.Services.GetService<IDomainRepository>();

            await repository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<PermissionValidationFailedException>();
        }

        [Fact]
        public async Task WhenSystemUser_Throws()
        {
            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = NEW_PASSWORD,
                OldPassword = OLD_PASSWORD
            };

            using var app = _appFactory.Create();

            // elevate to system user account
            var repository = app.Services
                .GetService<IDomainRepository>()
                .WithElevatedPermissions();

            await repository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<EntityNotFoundException<User>>();
        }

        [Fact]
        public async Task SendsMessage()
        {
            var username = "SendsMessage" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var loginService = app.Services.GetRequiredService<ILoginService>();

            await loginService.LogAuthenticatedUserInAsync(TestUserArea1.Code, userId, false);
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
            var username = "SendsMail" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var loginService = app.Services.GetRequiredService<ILoginService>();

            await loginService.LogAuthenticatedUserInAsync(TestUserArea1.Code, userId, false);
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
            var loginUrl = siteUrlResolver.MakeAbsolute(app.SeededEntities.TestUserArea1.Definition.LoginPath);

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
                    loginUrl
                )
                .Should().Be(1);
        }

        private async Task<int> AddUserIfNotExistsAsync(string username)
        {
            using var app = _appFactory.Create();
            var dbContext = app.Services.GetService<CofoundryDbContext>();

            var userId = await dbContext
                .Users
                .Where(u => u.UserAreaCode == TestUserArea1.Code && u.Username == username)
                .Select(u => u.UserId)
                .SingleOrDefaultAsync();

            if (userId > 0)
            {
                return userId;
            }

            var repository = app.Services
                .GetService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var testRole = await repository
                .Roles()
                .GetByCode(TestUserArea1RoleA.Code)
                .AsDetails()
                .ExecuteAsync();

            var command = new AddUserCommand()
            {
                Email = username,
                Password = OLD_PASSWORD,
                FirstName = "Test",
                LastName = "User",
                UserAreaCode = TestUserArea1.Code,
                RoleId = testRole.RoleId,
                RequirePasswordChange = true
            };

            return await repository
                .Users()
                .AddAsync(command);
        }
    }
}