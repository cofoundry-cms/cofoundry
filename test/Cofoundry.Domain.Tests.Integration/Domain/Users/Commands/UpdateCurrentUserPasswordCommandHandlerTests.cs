using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain.Tests.Integration
{
    [Collection(nameof(DbDependentFixture))]
    public class UpdateCurrentUserPasswordCommandHandlerTests
    {
        const string TEST_DOMAIN = "@UpdateCurrentUserPasswordCommandHandlerTests.example.com";
        const string OLD_PASSWORD = "Gr!sh3nk0";
        const string NEW_PASSWORD = "S3v3rn@ya";

        private readonly DbDependentFixture _dbDependentFixture;

        public UpdateCurrentUserPasswordCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
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

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                scope.MockDateTime(updateDate);

                var loginService = scope.GetService<ILoginService>();
                await loginService.LogAuthenticatedUserInAsync(TestUserArea1.Code, userId, false);

                var repository = scope.GetService<IDomainRepository>();
                await repository.ExecuteCommandAsync(command);
            }

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = username,
                Password = NEW_PASSWORD
            };

            UserLoginInfoAuthenticationResult result;
            User user = null;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                var dbContext = scope.GetService<CofoundryDbContext>();
                result = await repository.ExecuteQueryAsync(query);

                if (result?.User != null)
                {
                    user = await dbContext
                        .Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(u => u.UserId == result.User.UserId);
                }
            }

            Assert.NotNull(result);
            Assert.NotNull(result.User);
            Assert.False(result.User.RequirePasswordChange);
            Assert.False(result.User.IsEmailConfirmed);
            Assert.Equal(updateDate, user.LastPasswordChangeDate);
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

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                var loginService = scope.GetService<ILoginService>();
                await loginService.LogAuthenticatedUserInAsync(TestUserArea1.Code, userId, false);

                await Assert.ThrowsAsync<InvalidCredentialsAuthenticationException>(
                    () => repository.ExecuteCommandAsync(command)
                    );
            }
        }

        [Fact]
        public async Task WhenNotLoggedIn_Throws()
        {
            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = NEW_PASSWORD,
                OldPassword = OLD_PASSWORD
            };

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                await Assert.ThrowsAsync<PermissionValidationFailedException>(
                    () => repository.ExecuteCommandAsync(command)
                    );
            }
        }

        [Fact]
        public async Task WhenSystemUser_Throws()
        {
            var command = new UpdateCurrentUserPasswordCommand()
            {
                NewPassword = NEW_PASSWORD,
                OldPassword = OLD_PASSWORD
            };

            // elevate to system user account
            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                await Assert.ThrowsAsync<EntityNotFoundException<User>>(() => repository
                    .WithElevatedPermissions()
                    .ExecuteCommandAsync(command));
            }
        }

        private async Task<int> AddUserIfNotExistsAsync(string username)
        {
            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var dbContext = scope.GetService<CofoundryDbContext>();

                var userId = await dbContext
                    .Users
                    .Where(u => u.UserAreaCode == TestUserArea1.Code && u.Username == username)
                    .Select(u => u.UserId)
                    .SingleOrDefaultAsync();

                if (userId > 0)
                {
                    return userId;
                }

                var repository = scope.GetService<IAdvancedContentRepository>();

                var testRole = await repository
                    .Roles()
                    .GetByCode(TestUserArea1Role.Code)
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
                    .WithElevatedPermissions()
                    .Users()
                    .AddAsync(command);
            }
        }
    }
}
