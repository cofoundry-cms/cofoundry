using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetUserLoginInfoIfAuthenticatedQueryHandlerTests
    {
        const string TEST_DOMAIN = "@GetUserLoginInfoIfAuthenticatedQueryHandlerTests.example.com";
        const string VALID_USERNAME = "dade" + TEST_DOMAIN;
        const string VALID_PASSWORD = "-Z3r0c007-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetUserLoginInfoIfAuthenticatedQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        ")]
        [InlineData(VALID_USERNAME)]
        public async Task WhenUsernameNotExists_ReturnsInvalidCredentialsError(string username)
        {
            await AddUserIfNotExistsAsync();

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Username = username,
                Password = VALID_PASSWORD
            };

            using var app = _appFactory.Create();
            var repository = app.Services.GetService<IDomainRepository>();

            var result = await repository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.User.Should().BeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().Be(UserLoginInfoAuthenticationError.InvalidCredentials);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        ")]
        [InlineData("belf0rd")]
        public async Task WhenPasswordInvalid_ReturnsInvalidCredentialsError(string password)
        {
            await AddUserIfNotExistsAsync();

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = VALID_USERNAME,
                Password = password
            };

            using var app = _appFactory.Create();
            var repository = app.Services.GetService<IDomainRepository>();

            var result = await repository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.User.Should().BeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().Be(UserLoginInfoAuthenticationError.InvalidCredentials);
            }
        }

        [Fact]
        public async Task WhenSystemUser_ReturnsInvalidCredentials()
        {
            // Set the system user password to a known value, as it is 
            // set randomly during installation
            using (var app = _appFactory.Create())
            {
                var dbContext = app.Services.GetService<CofoundryDbContext>();
                var passwordCryptographyService = app.Services.GetService<IPasswordCryptographyService>();
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
                UserAreaCode = CofoundryAdminUserArea.Code,
                Username = "System",
                Password = VALID_PASSWORD
            };

            UserLoginInfoAuthenticationResult result;

            using (var app = _appFactory.Create())
            {
                var repository = app.Services.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.User.Should().BeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().Be(UserLoginInfoAuthenticationError.InvalidCredentials);
            }
        }

        [Fact]
        public async Task WhenDeletedUser_ReturnsInvalidCredentials()
        {
            var username = "WhenDeletedUser_ReturnsNull" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username);

            using (var app = _appFactory.Create())
            {
                var repository = app.Services.GetService<IDomainRepository>();
                await repository
                    .WithElevatedPermissions()
                    .ExecuteCommandAsync(new DeleteUserCommand(userId));
            }

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Username = username,
                Password = VALID_PASSWORD
            };

            UserLoginInfoAuthenticationResult result;

            using (var app = _appFactory.Create())
            {
                var repository = app.Services.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.User.Should().BeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().Be(UserLoginInfoAuthenticationError.InvalidCredentials);
            }
        }

        [Fact]
        public async Task WhenIncorrectUserArea_ReturnsInvalidCredentials()
        {
            await AddUserIfNotExistsAsync();

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea2.Code,
                Username = VALID_USERNAME,
                Password = VALID_PASSWORD
            };

            UserLoginInfoAuthenticationResult result;

            using (var app = _appFactory.Create())
            {
                var repository = app.Services.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.User.Should().BeNull();
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().Be(UserLoginInfoAuthenticationError.InvalidCredentials);
            }
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

            using var app = _appFactory.Create();
            var repository = app.Services.GetService<IDomainRepository>();

            var result = await repository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.User.Should().NotBeNull();
                result.User.UserId.Should().Be(userId);
                result.User.UserAreaCode.Should().Be(TestUserArea1.Code);
                result.IsSuccess.Should().BeTrue();
                result.Error.Should().Be(UserLoginInfoAuthenticationError.None);

                // Non-defaults for these props are covered in other tests
                result.User.RequirePasswordChange.Should().BeFalse();
                result.User.PasswordRehashNeeded.Should().BeFalse();
                result.User.IsEmailConfirmed.Should().BeFalse();
            }
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

            UserLoginInfoAuthenticationResult result;

            using var app = _appFactory.Create();
            var repository = app.Services.GetService<IDomainRepository>();

            result = await repository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.User.Should().NotBeNull();
                result.User.UserId.Should().Be(userId);
                result.IsSuccess.Should().BeTrue();
                result.Error.Should().Be(UserLoginInfoAuthenticationError.None);
                result.User.RequirePasswordChange.Should().BeTrue();
            }
        }

        [Fact]
        public async Task WhenOldPasswordHash_PasswordRehashNeededTrue()
        {
            var username = "WhenOldPasswordHash_PasswordRehashNeededTrue" + TEST_DOMAIN;
            var userId = await AddUserIfNotExistsAsync(username);

            using (var app = _appFactory.Create())
            {
                var dbContext = app.Services.GetService<CofoundryDbContext>();
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

            UserLoginInfoAuthenticationResult result;

            using (var app = _appFactory.Create())
            {
                var repository = app.Services.GetService<IDomainRepository>();
                result = await repository.ExecuteQueryAsync(query);
            }

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.User.Should().NotBeNull();
                result.User.UserId.Should().Be(userId);
                result.IsSuccess.Should().BeTrue();
                result.Error.Should().Be(UserLoginInfoAuthenticationError.None);
                result.User.PasswordRehashNeeded.Should().BeTrue();
            }
        }

        [Fact]
        public async Task WhenPasswordInvalid_SendsMessage()
        {
            await AddUserIfNotExistsAsync();

            using var app = _appFactory.Create();
            var repository = app.Services.GetService<IDomainRepository>();

            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = VALID_USERNAME,
                Password = "test"
            };
            await repository.ExecuteQueryAsync(query);

            app.Mocks
                .CountMessagesPublished<UserAuthenticationFailedMessage>(m =>
                {
                    return m.Username == VALID_USERNAME.ToLowerInvariant() && m.UserAreaCode == TestUserArea1.Code;
                })
                .Should().Be(1);
        }

        private async Task<int> AddUserIfNotExistsAsync(string username = VALID_USERNAME, Action<AddUserCommand> commandModifier = null)
        {
            if (commandModifier != null && username == VALID_USERNAME)
            {
                throw new ArgumentException("You must use a custom username if you are modifying the user command", nameof(username));
            }

            using var app = _appFactory.Create();
            var dbContext = app.Services.GetService<CofoundryDbContext>();
            var userArea = app.SeededEntities.TestUserArea1;

            var userId = await dbContext
                .Users
                .Where(u => u.UserAreaCode == userArea.UserAreaCode && u.Username == username)
                .Select(u => u.UserId)
                .SingleOrDefaultAsync();

            if (userId > 0)
            {
                return userId;
            }

            var repository = app.Services.GetService<IAdvancedContentRepository>();

            var command = new AddUserCommand()
            {
                Email = username,
                Password = VALID_PASSWORD,
                FirstName = "Test",
                LastName = "User",
                UserAreaCode = userArea.UserAreaCode,
                RoleId = userArea.RoleA.RoleId
            };

            commandModifier?.Invoke(command);

            return await repository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(command);
        }
    }
}
