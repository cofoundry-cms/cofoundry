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
    public class AddUserCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "AddUserCHT-";
        const string PASSWORD = "keepitsecretKeepitsaf3";
        const string EMAIL_DOMAIN = "@example.com";
        private readonly DbDependentTestApplicationFactory _appFactory;

        public AddUserCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task WhenPasswordSignIn_CanAddWithMinimalData()
        {
            var uniqueData = UNIQUE_PREFIX + "PWSignIn_AddMinData";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleCode = app.SeededEntities.TestUserArea1.RoleA.RoleCode,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .Include(r => r.Role)
                .Include(r => r.EmailDomain)
                .FilterById(command.OutputUserId)
                .SingleOrDefaultAsync();

            var lowerEmail = command.Email.ToLowerInvariant();

            using (new AssertionScope())
            {
                command.OutputUserId.Should().BePositive();
                user.Should().NotBeNull();
                user.FirstName.Should().BeNull();
                user.LastName.Should().BeNull();
                user.CreateDate.Should().NotBeDefault();
                user.CreatorId.Should().BePositive();
                user.Email.Should().Be(command.Email);
                user.UniqueEmail.Should().Be(lowerEmail);
                user.DeactivatedDate.Should().BeNull();
                user.DeletedDate.Should().BeNull();
                user.AccountVerifiedDate.Should().BeNull();
                user.IsSystemAccount.Should().BeFalse();
                user.LastSignInDate.Should().BeNull();
                user.LastPasswordChangeDate.Should().NotBeDefault();
                user.Password.Should().NotBeNullOrWhiteSpace();
                user.PasswordHashVersion.Should().BePositive();
                user.PreviousSignInDate.Should().NotBeDefault();
                user.RequirePasswordChange.Should().BeFalse();
                user.Role.RoleCode.Should().Be(command.RoleCode);
                user.RoleId.Should().BePositive();
                user.UserAreaCode.Should().Be(command.UserAreaCode);
                user.Username.Should().Be(command.Email);
                user.UniqueUsername.Should().Be(lowerEmail);
                user.EmailDomain.Name.Should().Be("example.com");
                user.SecurityStamp.Should().NotBeNullOrEmpty().And.HaveLength(32);
            }
        }

        [Fact]
        public async Task CanAddWithRoleId()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanAddWithRoleId);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var userAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .Include(r => r.Role)
                .FilterById(command.OutputUserId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                command.OutputUserId.Should().BePositive();
                user.Should().NotBeNull();
                user.RoleId.Should().Be(roleId);
            }
        }

        [Fact]
        public async Task CanRequirePasswordChange()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanRequirePasswordChange);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userArea = app.SeededEntities.TestUserArea1;

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode,
                RequirePasswordChange = true
            };

            await contentRepository
                .Users()
                .AddAsync(command);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(command.OutputUserId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                command.OutputUserId.Should().BePositive();
                user.Should().NotBeNull();
                user.RequirePasswordChange.Should().BeTrue();
            }
        }

        [Fact]
        public async Task CanSetVerifiedTrue()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSetVerifiedTrue);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userArea = app.SeededEntities.TestUserArea1;

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode,
                IsAccountVerified = true
            };

            await contentRepository
                .Users()
                .AddAsync(command);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(command.OutputUserId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                command.OutputUserId.Should().BePositive();
                user.Should().NotBeNull();
                user.AccountVerifiedDate.Should().NotBeNull().And.NotBeDefault();
            }
        }

        [Fact]
        public async Task WithPasswordSignIn_PasswordRequired()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithPasswordSignIn_PasswordRequired);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userArea = app.SeededEntities.TestUserArea1;

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode
            };

            await contentRepository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(command.Password))
                .WithMessage("*required*");
        }

        [Fact]
        public async Task WithEmailAsUsername_EmailRequired()
        {
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userArea = app.SeededEntities.TestUserArea1;

            var command = new AddUserCommand()
            {
                Password = PASSWORD,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode
            };

            await contentRepository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(command.Email))
                .WithMessage("*required*");
        }

        [Fact]
        public async Task WithEmailAsUsername_UsernameShouldBeNull()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithEmailAsUsername_UsernameShouldBeNull);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userArea = app.SeededEntities.TestUserArea1;

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Username = uniqueData,
                Password = PASSWORD,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode
            };

            await contentRepository
                .Awaiting(r => r.Users().AddAsync(command))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(command.Username))
                .WithMessage("Username*empty*");
        }

        [Fact]
        public async Task WithoutEmailAsUsername_CanAddEmailAndUsername()
        {
            var uniqueData = UNIQUE_PREFIX + "WOEmailUNCanAdd";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Username = uniqueData,
                Password = PASSWORD,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(command.OutputUserId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                command.OutputUserId.Should().BePositive();
                user.Should().NotBeNull();
                user.Username.Should().Be(command.Username);
                user.Email.Should().Be(command.Email);
            }
        }

        [Fact]
        public async Task WithoutEmailAsUsername_UsernameRequired()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithoutEmailAsUsername_UsernameRequired);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Awaiting(r => r.Users().AddAsync(command))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(command.Username))
                .WithMessage("*required*");
        }

        [Fact]
        public async Task WithEmailAsUsername_ValidatesEmailUnique()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithEmailAsUsername_ValidatesEmailUnique);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userArea = app.SeededEntities.TestUserArea1;

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);
            command.OutputUserId = 0;

            await contentRepository
                .Awaiting(r => r.Users().AddAsync(command))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(command.Email))
                .WithMessage("*email*already*registered*");
        }

        [Fact]
        public async Task WithoutEmailAsUsername_ValidatesUsernameUnique()
        {
            var uniqueData = UNIQUE_PREFIX + "WOEmailUN_ValUnameUnique";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Username = uniqueData,
                Password = PASSWORD,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);

            command.Email = uniqueData + "2" + EMAIL_DOMAIN;
            command.OutputUserId = 0;

            await contentRepository
                .Awaiting(r => r.Users().AddAsync(command))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(command.Username))
                .WithMessage("*username*already*registered*");
        }

        [Fact]
        public async Task WhenNotPasswordSignIn_EmailNotRequired()
        {
            var uniqueData = UNIQUE_PREFIX + "NotPWSignIn_EmailNotReq";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userAreaCode = UserAreaWithoutPasswordSignIn.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserCommand()
            {
                Username = uniqueData,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(command.OutputUserId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                command.OutputUserId.Should().BePositive();
                user.Should().NotBeNull();
                user.Username.Should().Be(command.Username);
                user.Email.Should().BeNull();
            }
        }

        [Fact]
        public async Task WhenEmailRequiredUnique_ValidatesEmailUnique()
        {
            var uniqueData = UNIQUE_PREFIX + "EmailReqUniq_ValEmailUnique";

            var userSettings = new UsersSettings();
            userSettings.EmailAddress.RequireUnique = true;
            using var app = _appFactory.Create(s =>
            {
                s.AddSingleton(userSettings);
            });

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Username = uniqueData,
                Password = PASSWORD,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);
            command.Username = uniqueData + "2";
            command.OutputUserId = 0;

            await contentRepository
                .Awaiting(r => r.Users().AddAsync(command))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithMemberNames(nameof(command.Email))
                .WithMessage("*email*already*registered*");
        }

        [Fact]
        public async Task WhenEmailNotRequiredUnique_CanAddSameEmail()
        {
            var uniqueData = UNIQUE_PREFIX + "EmailNotReqUniq_CanAddSameEmail";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Username = uniqueData,
                Password = PASSWORD,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);
            command.Username = uniqueData + "2";
            command.OutputUserId = 0;

            await contentRepository
                .Users()
                .AddAsync(command);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(command.OutputUserId)
                .SingleOrDefaultAsync();

            user.Should().NotBeNull();
        }

        [Fact]
        public async Task SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(SendsMessage);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var userArea = app.SeededEntities.TestUserArea1;

            var command = new AddUserCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode
            };

            await contentRepository
                .Users()
                .AddAsync(command);

            using (new AssertionScope())
            {
                app.Mocks
                    .CountMessagesPublished<UserAddedMessage>(m => m.UserId == command.OutputUserId && m.UserAreaCode == userArea.UserAreaCode)
                    .Should().Be(1);

                app.Mocks
                    .CountMessagesPublished<UserUsernameUpdatedMessage>()
                    .Should().Be(0);

                app.Mocks
                    .CountMessagesPublished<UserEmailUpdatedMessage>()
                    .Should().Be(0);
            }
        }
    }
}
