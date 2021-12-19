using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using Cofoundry.Domain.Tests.Shared.SeedData;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class UpdateCurrentUserAccountCommandHandlerTests
    {
        private const string UNIQUE_PREFIX = "UpdCurUsrAccCHT-";
        private const string PASSWORD = "neverbr3@kthechange";
        private static string EMAIL_DOMAIN = $"@{UNIQUE_PREFIX}.example.com";
        private readonly DbDependentTestApplicationFactory _appFactory;

        public UpdateCurrentUserAccountCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanUpdateName()
        {
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var userArea = app.SeededEntities.TestUserArea1;

            var addCommand = new AddUserCommand()
            {
                Email = "reginald.dwight" + EMAIL_DOMAIN,
                Password = PASSWORD,
                FirstName = "Reginald",
                LastName = "Dwight",
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode
            };

            var userId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addCommand);

            await loginService.LogAuthenticatedUserInAsync(userArea.UserAreaCode, userId, true);

            var updateCommand = new UpdateCurrentUserAccountCommand()
            {
                Email = addCommand.Email,
                FirstName = "Elton",
                LastName = "John"
            };

            await contentRepository
                .Users()
                .UpdateCurrentUserAccountAsync(updateCommand);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                user.Should().NotBeNull();
                user.FirstName.Should().Be(updateCommand.FirstName);
                user.LastName.Should().Be(updateCommand.LastName);
                user.Email.Should().Be(addCommand.Email.ToLowerInvariant());
            }
        }

        [Fact]
        public async Task CanUpdateEmail()
        {
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var userArea = app.SeededEntities.TestUserArea1;

            var addCommand = new AddUserCommand()
            {
                Email = $"prince" + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode
            };

            var userId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addCommand);

            await loginService.LogAuthenticatedUserInAsync(userArea.UserAreaCode, userId, true);

            var newEmailDomain = $"{UNIQUE_PREFIX}2.example.com";
            var newEmailLocal = "TAFKAP@";
            var updateCommand = new UpdateCurrentUserAccountCommand()
            {
                Email = newEmailLocal + newEmailDomain,
            };

            await contentRepository
                .Users()
                .UpdateCurrentUserAccountAsync(updateCommand);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .Include(u => u.EmailDomain)
                .FilterById(userId)
                .SingleOrDefaultAsync();

            var normalizedDomain = newEmailDomain.ToLowerInvariant();
            var newNormalizedEmail = newEmailLocal + normalizedDomain;
            var newEmailLower = newNormalizedEmail.ToLowerInvariant();
            using (new AssertionScope())
            {
                user.Should().NotBeNull();
                user.FirstName.Should().Be(updateCommand.FirstName);
                user.LastName.Should().Be(updateCommand.LastName);
                user.Email.Should().Be(newNormalizedEmail);
                user.UniqueEmail.Should().Be(newEmailLower);
                user.Username.Should().Be(newNormalizedEmail);
                user.UniqueUsername.Should().Be(newEmailLower);
                user.EmailDomain.Name.Should().Be(normalizedDomain);
            }
        }

        [Fact]
        public async Task CanUpdateUsername()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanUpdateUsername);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var addCommand = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Username = "NWatkins",
                Password = PASSWORD,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            var userId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addCommand);

            await loginService.LogAuthenticatedUserInAsync(userAreaCode, userId, true);

            var updateCommand = new UpdateCurrentUserAccountCommand()
            {
                Email = addCommand.Email,
                Username = "TBone"
            };

            await contentRepository
                .Users()
                .UpdateCurrentUserAccountAsync(updateCommand);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                user.Should().NotBeNull();
                user.Username.Should().Be(updateCommand.Username);
                user.UniqueUsername.Should().Be(updateCommand.Username.ToLowerInvariant());
            }
        }

        [Fact]
        public async Task CanUnsetData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanUnsetData);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var userAreaCode = UserAreaWithoutPasswordLogin.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var addCommand = new AddUserCommand()
            {
                Email = $"e.razor" + EMAIL_DOMAIN,
                Username = uniqueData,
                FirstName = "John",
                LastName = "Kruger",
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            var userId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addCommand);

            await loginService.LogAuthenticatedUserInAsync(userAreaCode, userId, true);

            await contentRepository
                .Users()
                .UpdateCurrentUserAccountAsync(new UpdateCurrentUserAccountCommand()
                {
                    Username = addCommand.Username
                });

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                user.Should().NotBeNull();
                user.FirstName.Should().BeNull();
                user.LastName.Should().BeNull();
                user.Email.Should().BeNull();
                user.UniqueEmail.Should().BeNull();
                user.EmailDomainId.Should().BeNull();
                user.Username.Should().Be(addCommand.Username);
                user.UniqueUsername.Should().Be(addCommand.Username.ToLowerInvariant());
            }
        }

        [Fact]
        public async Task WhenUsernameNotUnique_Throws()
        {
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var userArea = app.SeededEntities.TestUserArea1;

            await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(new AddUserCommand()
                {
                    Email = $"djones" + EMAIL_DOMAIN,
                    Password = PASSWORD,
                    RoleCode = userArea.RoleA.RoleCode,
                    UserAreaCode = userArea.UserAreaCode
                });

            var userId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(new AddUserCommand()
                {
                    Email = $"dbowie" + EMAIL_DOMAIN,
                    Password = PASSWORD,
                    RoleCode = userArea.RoleA.RoleCode,
                    UserAreaCode = userArea.UserAreaCode
                });

            var updateCommand = new UpdateCurrentUserAccountCommand()
            {
                Email = $"djones" + EMAIL_DOMAIN,
            };

            await loginService.LogAuthenticatedUserInAsync(userArea.UserAreaCode, userId, true);

            await contentRepository
                .Awaiting(r => r.Users().UpdateCurrentUserAccountAsync(updateCommand))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(updateCommand.Email))
                .WithMessage("*already registered*");
        }


        [Fact]
        public async Task WhenEmailNotUnique_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenEmailNotUnique_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var loginService = app.Services.GetRequiredService<ILoginService>();
            var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(new AddUserCommand()
                {
                    Email = uniqueData + EMAIL_DOMAIN,
                    Username = uniqueData,
                    Password = PASSWORD,
                    RoleId = roleId,
                    UserAreaCode = userAreaCode
                });

            var userId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(new AddUserCommand()
                {
                    Email = uniqueData + "2" + EMAIL_DOMAIN,
                    Username = uniqueData + "2",
                    Password = PASSWORD,
                    RoleId = roleId,
                    UserAreaCode = userAreaCode
                });

            var updateCommand = new UpdateCurrentUserAccountCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
            };

            await loginService.LogAuthenticatedUserInAsync(userAreaCode, userId, true);

            await contentRepository
                .Awaiting(r => r.Users().UpdateCurrentUserAccountAsync(updateCommand))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(updateCommand.Email))
                .WithMessage("*already registered*");
        }

        [Fact]
        public async Task WhenSystemUser_Throws()
        {
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();

            await contentRepository
                .Awaiting(r => r.WithElevatedPermissions().Users().UpdateCurrentUserAccountAsync(new UpdateCurrentUserAccountCommand()))
                .Should()
                .ThrowAsync<EntityNotFoundException>();
        }
    }
}
