using Cofoundry.Core;
using Cofoundry.Core.Validation;
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
    public class LogUserInWithCredentialsCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "LogInWCredCHT-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        const string PASSWORD = "(>\")><(\"<)";

        public LogUserInWithCredentialsCommandHandlerTests(
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

            var user = await AddUser(app, uniqueData);

            await contentRepository
                .Users()
                .Authentication()
                .LogUserInWithCredentialsAsync(new LogUserInWithCredentialsCommand()
                {
                    UserAreaCode = user.UserAreaCode,
                    Password = PASSWORD,
                    Username = user.Username
                });


            var sessionUserId = await userSessionService.GetUserIdByUserAreaCodeAsync(user.UserAreaCode);

            sessionUserId.Should().Be(sessionUserId);
        }

        [Fact]
        public async Task WhenInvalid_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "InvalidThr";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var user = await AddUser(app, uniqueData);

            await contentRepository
                .Awaiting(r => r.Users().Authentication().LogUserInWithCredentialsAsync(new LogUserInWithCredentialsCommand()
                {
                    UserAreaCode = user.UserAreaCode,
                    Password = "wibble",
                    Username = user.Username
                }))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithErrorCode(UserValidationErrors.Authentication.InvalidCredentials.ErrorCode);
        }

        [Fact]
        public async Task WhenPasswordChangeRequired_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "PWChangeReq";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var user = await AddUser(app, uniqueData, c =>
            {
                c.RequirePasswordChange = true;
            });

            await contentRepository
                .Awaiting(r => r.Users().Authentication().LogUserInWithCredentialsAsync(new LogUserInWithCredentialsCommand()
                {
                    UserAreaCode = user.UserAreaCode,
                    Password = PASSWORD,
                    Username = user.Username
                }))
                .Should()
                .ThrowAsync<PasswordChangeRequiredException>()
                .WithErrorCode(UserValidationErrors.Authentication.PasswordChangeRequired.ErrorCode);
        }

        [Fact]
        public async Task WhenVerificationRequired_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "VerReq";

            using var app = _appFactory.Create(s => s.Configure<UsersSettings>(u => u.AccountVerification.RequireVerification = true));
            var contentRepository = app.Services.GetContentRepository();
            var user = await AddUser(app, uniqueData);

            await contentRepository
                .Awaiting(r => r.Users().Authentication().LogUserInWithCredentialsAsync(new LogUserInWithCredentialsCommand()
                {
                    UserAreaCode = user.UserAreaCode,
                    Password = PASSWORD,
                    Username = user.Username
                }))
                .Should()
                .ThrowAsync<AccountNotVerifiedException>()
                .WithErrorCode(UserValidationErrors.Authentication.AccountNotVerified.ErrorCode);
        }

        [Fact]
        public async Task WhenVerificationRequired_CanLogInVerifiedUser()
        {
            var uniqueData = UNIQUE_PREFIX + "VerReqVerOk";

            using var app = _appFactory.Create(s => s.Configure<UsersSettings>(u => u.AccountVerification.RequireVerification = true));
            var contentRepository = app.Services.GetContentRepository();
            var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
            var user = await AddUser(app, uniqueData, c => c.IsAccountVerified = true);

            await contentRepository
                .Users()
                .Authentication()
                .LogUserInWithCredentialsAsync(new LogUserInWithCredentialsCommand()
                {
                    UserAreaCode = user.UserAreaCode,
                    Password = PASSWORD,
                    Username = user.Username
                });


            var sessionUserId = await userSessionService.GetUserIdByUserAreaCodeAsync(user.UserAreaCode);

            sessionUserId.Should().Be(sessionUserId);
        }

        private static async Task<User> AddUser(DbDependentTestApplication app, string uniqueData, Action<AddUserCommand> configureCommand = null)
        {
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var userId = await app.TestData.Users().AddAsync(uniqueData, c => {
                c.Password = PASSWORD;
                configureCommand?.Invoke(c);
            });

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleAsync();
            return user;
        }
    }
}
