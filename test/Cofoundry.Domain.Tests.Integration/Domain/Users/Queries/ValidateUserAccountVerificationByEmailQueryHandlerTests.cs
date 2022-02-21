using Cofoundry.Domain.CQS;
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
    public class ValidateUserAccountVerificationByEmailQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "ValAccVerQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public ValidateUserAccountVerificationByEmailQueryHandlerTests(
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

            var seedDate = new DateTime(1952, 01, 20);
            var authorizedTask = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeToken(authorizedTask);

            app.Mocks.MockDateTime(seedDate.AddHours(10));
            var result = await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .Validate(new ValidateUserAccountVerificationByEmailQuery()
                {
                    UserAreaCode = authorizedTask.User.UserAreaCode,
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
            var seedDate = new DateTime(1952, 02, 20);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            app.Mocks.MockDateTime(seedDate);
            var authorizedTask = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeToken(authorizedTask);

            app.Mocks.MockDateTime(seedDate.AddDays(7).AddHours(1));
            var result = await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .Validate(new ValidateUserAccountVerificationByEmailQuery()
                {
                    UserAreaCode = authorizedTask.User.UserAreaCode,
                    Token = token,
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountVerification.RequestValidation.Expired.ErrorCode);
            }
        }

        [Fact]
        public async Task WhenAlreadyComplete_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WComplete_Err";
            var seedDate = new DateTime(2052, 04, 20);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var authorizedTask = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeToken(authorizedTask);

            await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .CompleteAsync(new CompleteUserAccountVerificationByEmailCommand()
                {
                    UserAreaCode = authorizedTask.User.UserAreaCode,
                    Token = token
                });

            var result = await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .Validate(new ValidateUserAccountVerificationByEmailQuery()
                {
                    UserAreaCode = authorizedTask.User.UserAreaCode,
                    Token = token
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountVerification.RequestValidation.AlreadyComplete.ErrorCode);
            }
        }

        [Fact]
        public async Task WhenInvalid_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WInv_Err";
            var seedDate = new DateTime(1952, 05, 20);

            using var app = _appFactory.Create();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var authorizedTask = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeToken(authorizedTask);
            authorizedTask.InvalidatedDate = seedDate;
            await dbContext.SaveChangesAsync();

            var result = await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .Validate(new ValidateUserAccountVerificationByEmailQuery()
                {
                    UserAreaCode = authorizedTask.User.UserAreaCode,
                    Token = token,
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountVerification.RequestValidation.Invalidated.ErrorCode);
            }
        }

        [Fact]
        public async Task WhenNotExists_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WNotEx_Err";
            var seedDate = new DateTime(1952, 06, 20);

            using var app = _appFactory.Create();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var authorizedTask = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeToken(authorizedTask);

            var result = await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .Validate(new ValidateUserAccountVerificationByEmailQuery()
                {
                    UserAreaCode = authorizedTask.User.UserAreaCode,
                    Token = token + "X",
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountVerification.RequestValidation.NotFound.ErrorCode);
            }
        }

        [Fact]
        public async Task WhenEmailChanged_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WEmailChg_Err";
            var seedDate = new DateTime(1952, 06, 20);

            using var app = _appFactory.Create();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var authorizedTask = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeToken(authorizedTask);

            var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdateUserCommand>(authorizedTask.UserId));
            updateCommand.Email = uniqueData + "@2.example.com";
            await contentRepository.Users().UpdateAsync(updateCommand);

            var result = await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .Validate(new ValidateUserAccountVerificationByEmailQuery()
                {
                    UserAreaCode = authorizedTask.User.UserAreaCode,
                    Token = token,
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(UserValidationErrors.AccountVerification.RequestValidation.EmailMismatch.ErrorCode);
            }
        }

        private static string MakeToken(AuthorizedTask authorizedTask)
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

            var userId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addUserCommand);

            await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .InitiateAsync(new InitiateUserAccountVerificationByEmailCommand()
                {
                    UserId = userId
                });

            var authorizedTask = await dbContext
                .AuthorizedTasks
                .Include(u => u.User)
                .SingleAsync(u => u.UserId == addUserCommand.OutputUserId);

            return authorizedTask;
        }
    }
}
