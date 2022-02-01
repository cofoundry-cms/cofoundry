using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Core.Web;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;


namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class UpdateUserPasswordByCredentialsCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "UpdUsrPWCredCHT ";
        private readonly DbDependentTestApplicationFactory _appFactory;

        public UpdateUserPasswordByCredentialsCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task WhenValid_ChangesPassword()
        {
            var uniqueData = UNIQUE_PREFIX + "Valid_ChangesPW";
            var newPassword = uniqueData + "New";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var addUserCommand = app.TestData.Users().CreateAddCommand(uniqueData);
            await contentRepository.Users().AddAsync(addUserCommand);

            var command = new UpdateUserPasswordByCredentialsCommand()
            {
                Username = addUserCommand.Email,
                NewPassword = newPassword,
                OldPassword = addUserCommand.Password,
                UserAreaCode = addUserCommand.UserAreaCode
            };
            await contentRepository.Users().UpdatePasswordByCredentialsAsync(command);

            // Use the auth query to verify the password has been changed
            var authResult = await contentRepository.ExecuteQueryAsync(new ValidateUserCredentialsQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Username = command.Username,
                Password = newPassword
            });

            using (new AssertionScope())
            {
                authResult.Should().NotBeNull();
                authResult.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public async Task WhenCredentialsInvalid_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "CredInv_Throws";
            var newPassword = uniqueData + "New";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var addUserCommand = app.TestData.Users().CreateAddCommand(uniqueData);
            await contentRepository.Users().AddAsync(addUserCommand);

            var command = new UpdateUserPasswordByCredentialsCommand()
            {
                Username = addUserCommand.Email,
                NewPassword = newPassword,
                OldPassword = "invalid",
                UserAreaCode = addUserCommand.UserAreaCode
            };
            await contentRepository
                .Awaiting(r => r.Users().UpdatePasswordByCredentialsAsync(command))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithErrorCode(UserValidationErrors.Authentication.InvalidCredentials.ErrorCode);
        }

        [Fact]
        public async Task SendsMail()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(SendsMail);
            var newPassword = uniqueData + "New";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var addUserCommand = app.TestData.Users().CreateAddCommand(uniqueData);
            await contentRepository.Users().AddAsync(addUserCommand);

            var command = new UpdateUserPasswordByCredentialsCommand()
            {
                Username = addUserCommand.Email,
                NewPassword = newPassword,
                OldPassword = addUserCommand.Password,
                UserAreaCode = addUserCommand.UserAreaCode
            };
            await contentRepository.Users().UpdatePasswordByCredentialsAsync(command);

            var siteUrlResolver = app.Services.GetRequiredService<ISiteUrlResolver>();
            var loginUrl = siteUrlResolver.MakeAbsolute(app.SeededEntities.TestUserArea1.Definition.LoginPath);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(addUserCommand.OutputUserId)
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
    }
}
