using Cofoundry.Domain.Data;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Domain.Users.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class ValidateUserAccountRecoveryByEmailQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "ValAccRecQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public ValidateUserAccountRecoveryByEmailQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task Valid_ReturnsValid()
        {
            var uniqueData = UNIQUE_PREFIX + "Valid";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var seedDate = new DateTime(2022, 01, 20);
            var request = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeResetToken(request);

            app.Mocks.MockDateTime(seedDate.AddHours(10));
            var result = await contentRepository
                .Users()
                .AccountRecovery()
                .Validate(new ValidateUserAccountRecoveryByEmailQuery()
                {
                    UserAreaCode = request.User.UserAreaCode,
                    Token = token,
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                result.Error.Should().BeNull();
            }
        }

        [Fact]
        public async Task WhenExpired_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WExpired_Err";
            var seedDate = new DateTime(2021, 01, 20);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            app.Mocks.MockDateTime(seedDate);
            var request = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeResetToken(request);

            app.Mocks.MockDateTime(seedDate.AddHours(17));
            var result = await contentRepository
                .Users()
                .AccountRecovery()
                .Validate(new ValidateUserAccountRecoveryByEmailQuery()
                {
                    UserAreaCode = request.User.UserAreaCode,
                    Token = token,
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountRecovery.RequestValidation.Expired.ErrorCode);
            }
        }

        [Fact]
        public async Task WhenAlreadyComplete_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WComplete_Err";
            var seedDate = new DateTime(2019, 01, 20);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var request = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeResetToken(request);

            await contentRepository
                .Users()
                .AccountRecovery()
                .CompleteAsync(new CompleteUserAccountRecoveryByEmailCommand()
                {
                    UserAreaCode = request.User.UserAreaCode,
                    Token = token,
                    NewPassword = uniqueData
                });

            var result = await contentRepository
                .Users()
                .AccountRecovery()
                .Validate(new ValidateUserAccountRecoveryByEmailQuery()
                {
                    UserAreaCode = request.User.UserAreaCode,
                    Token = token,
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountRecovery.RequestValidation.AlreadyComplete.ErrorCode);
            }
        }

        [Fact]
        public async Task WhenInvalid_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WInv_Err";
            var seedDate = new DateTime(2018, 01, 20);

            using var app = _appFactory.Create();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var request = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeResetToken(request);
            request.InvalidatedDate = seedDate;
            await dbContext.SaveChangesAsync();

            var result = await contentRepository
                .Users()
                .AccountRecovery()
                .Validate(new ValidateUserAccountRecoveryByEmailQuery()
                {
                    UserAreaCode = request.User.UserAreaCode,
                    Token = token,
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountRecovery.RequestValidation.Invalidated.ErrorCode);
            }
        }

        [Fact]
        public async Task WhenNotExists_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WNotEx_Err";
            var seedDate = new DateTime(2017, 01, 20);

            using var app = _appFactory.Create();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var request = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeResetToken(request);

            var result = await contentRepository
                .Users()
                .AccountRecovery()
                .Validate(new ValidateUserAccountRecoveryByEmailQuery()
                {
                    UserAreaCode = request.User.UserAreaCode,
                    Token = token + "X",
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountRecovery.RequestValidation.NotFound.ErrorCode);
            }
        }

        private static string MakeResetToken(AuthorizedTask authorizedTask)
        {
            var formatter = new AuthorizedTaskTokenFormatter();
            return formatter.Format(new AuthorizedTaskTokenParts()
            {
                AuthorizationCode = authorizedTask.AuthorizationCode,
                AuthorizedTaskId = authorizedTask.AuthorizedTaskId
            });
        }

        private static async Task<AuthorizedTask> AddUserAndInitiateRequest(
            string uniqueData,
            DbDependentTestApplication app
            )
        {
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var addUserCommand = app.TestData.Users().CreateAddCommand(uniqueData);

            await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addUserCommand);

            await contentRepository
                .Users()
                .AccountRecovery()
                .InitiateAsync(new InitiateUserAccountRecoveryByEmailCommand()
                {
                    UserAreaCode = addUserCommand.UserAreaCode,
                    Username = addUserCommand.Email
                });

            var authorizedTask = await dbContext
                .AuthorizedTasks
                .Include(u => u.User)
                .SingleAsync(u => u.UserId == addUserCommand.OutputUserId);

            return authorizedTask;
        }
    }
}
