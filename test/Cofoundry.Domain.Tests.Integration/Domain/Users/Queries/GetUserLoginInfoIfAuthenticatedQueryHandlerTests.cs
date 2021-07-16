using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Cofoundry.Domain.Tests.Shared;

namespace Cofoundry.Domain.Tests.Integration
{
    [Collection(nameof(DbDependentFixture))]
    public class GetUserLoginInfoIfAuthenticatedQueryHandlerTests
    {
        const string TEST_DOMAIN = "@GetUserLoginInfoIfAuthenticatedQueryHandlerTests.example.com";
        const string VALID_USERNAME = "dade" + TEST_DOMAIN;
        const string VALID_PASSWORD = "Z3r0c007";

        private readonly DbDependentFixture _dbDependentFixture;

        public GetUserLoginInfoIfAuthenticatedQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        ")]
        [InlineData(VALID_USERNAME)]
        public async Task WhenUsernameNotExists_ReturnsNull(string username)
        {
            await AddUserIfNotExistsAsync();

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.AreaCode,
                Username = username,
                Password = VALID_PASSWORD
            };

            UserLoginInfo result;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            Assert.Null(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        ")]
        [InlineData("belf0rd")]
        public async Task WhenPasswordInvalid_ReturnsNull(string password)
        {
            await AddUserIfNotExistsAsync();

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = VALID_USERNAME,
                Password = password
            };

            UserLoginInfo result;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenSystemUser_ReturnsNull()
        {
            // Set the system user password to a known value, as it is 
            // set randomly during installation
            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var dbContext = scope.GetService<CofoundryDbContext>();
                var passwordCryptographyService = scope.GetService<IPasswordCryptographyService>();
                var systemUser = await dbContext
                    .Users
                    .SingleAsync(u => u.IsSystemAccount);

                var hash = passwordCryptographyService.CreateHash(VALID_PASSWORD);
                systemUser.Password = hash.Hash;
                systemUser.PasswordHashVersion = hash.HashVersion;

                await dbContext.SaveChangesAsync();
            }

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.AreaCode,
                Username = "System",
                Password = VALID_PASSWORD
            };

            UserLoginInfo result;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenDeletedUser_ReturnsNull()
        {
            var username = "WhenDeletedUser_ReturnsNull" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username);

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                await repository
                    .WithElevatedPermissions()
                    .ExecuteCommandAsync(new DeleteUserCommand(userId));
            }

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.AreaCode,
                Username = username,
                Password = VALID_PASSWORD
            };

            UserLoginInfo result;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenIncorrectUserArea_ReturnsNull()
        {
            await AddUserIfNotExistsAsync();

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea2.Code,
                Username = VALID_USERNAME,
                Password = VALID_PASSWORD
            };

            UserLoginInfo result;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenValid_Maps()
        {
            var userId = await AddUserIfNotExistsAsync();

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = VALID_USERNAME,
                Password = VALID_PASSWORD
            };

            UserLoginInfo result;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            Assert.Equal(userId, result.UserId);
            Assert.Equal(TestUserArea1.Code, result.UserAreaCode);

            // Non-defaults for these props are covered in other tests
            Assert.False(result.RequirePasswordChange);
            Assert.False(result.PasswordRehashNeeded);
        }

        [Fact]
        public async Task WhenPasswordChangeRequired_RequirePasswordChangeTrue()
        {
            var username = "WhenPasswordChangeRequired_RequirePasswordChangeTrue" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username, c => c.RequirePasswordChange = true);

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = username,
                Password = VALID_PASSWORD
            };

            UserLoginInfo result;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            Assert.Equal(userId, result.UserId);
            Assert.True(result.RequirePasswordChange);
        }

        [Fact]
        public async Task WhenOldPasswordHash_PasswordRehashNeededTrue()
        {
            var username = "WhenOldPasswordHash_PasswordRehashNeededTrue" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username);

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var dbContext = scope.GetService<CofoundryDbContext>();
                var user = await dbContext
                    .Users
                    .SingleAsync(u => u.UserId == userId);

                user.Password = Defuse.PasswordCryptographyV2.CreateHash(VALID_PASSWORD);
                user.PasswordHashVersion = (int)PasswordHashVersion.V2;

                await dbContext.SaveChangesAsync();
            }

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = username,
                Password = VALID_PASSWORD
            };

            UserLoginInfo result;

            using (var scope = _dbDependentFixture.CreateServiceScope())
            {
                var repository = scope.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            Assert.Equal(userId, result.UserId);
            Assert.True(result.PasswordRehashNeeded);
        }

        private async Task<int> AddUserIfNotExistsAsync(string username = VALID_USERNAME, Action<AddUserCommand> commandModifier = null)
        {
            if (commandModifier != null && username == VALID_USERNAME)
            {
                throw new ArgumentException("You must use a custom username if you are modifying the user command", nameof(username));
            }

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
                    Password = VALID_PASSWORD,
                    FirstName = "Test",
                    LastName = "User",
                    UserAreaCode = TestUserArea1.Code,
                    RoleId = testRole.RoleId
                };

                commandModifier?.Invoke(command);

                return await repository
                    .WithElevatedPermissions()
                    .Users()
                    .AddAsync(command);
            }
        }
    }
}
