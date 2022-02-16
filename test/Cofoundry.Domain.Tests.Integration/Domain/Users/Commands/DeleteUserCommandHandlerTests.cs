using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class DeleteUserCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DelUserCHT-";
        private readonly DbDependentTestApplicationFactory _appFactory;

        public DeleteUserCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanDelete()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanDelete);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var userId = await app.TestData.Users().AddAsync(uniqueData);
            var originalUser = await GetUserAsync(dbContext, userId);

            await contentRepository
                .Users()
                .DeleteAsync(userId);

            var deletedUser = await GetUserAsync(dbContext, userId);

            using (new AssertionScope())
            {
                deletedUser.Should().NotBeNull();
                deletedUser.Email.Should().BeNull();
                deletedUser.UniqueEmail.Should().BeNull();
                deletedUser.FirstName.Should().BeNull();
                deletedUser.LastName.Should().BeNull();
                deletedUser.EmailDomainId.Should().BeNull();
                deletedUser.Password.Should().NotBeNull().And.NotBe(originalUser.Username);
                deletedUser.Username.Should().NotBeNull().And.NotBe(originalUser.Username);
                deletedUser.UniqueUsername.Should().NotBeNull().And.NotBe(originalUser.UniqueUsername);
                deletedUser.DeactivatedDate.Should().NotBeNull();
                deletedUser.DeletedDate.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task WhenEmailDomainOrphaned_Deletes()
        {
            var uniqueData = UNIQUE_PREFIX + "EDOrph-Del";
            var uniqueDomain = uniqueData + ".co";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var userId = await app.TestData.Users().AddAsync(uniqueData, uniqueDomain);
            var originalUser = await GetUserAsync(dbContext, userId);

            await contentRepository
                .Users()
                .DeleteAsync(userId);

            var emailDomainExists = await dbContext
                .EmailDomains
                .AsNoTracking()
                .AnyAsync(e => e.EmailDomainId == originalUser.EmailDomainId);

            emailDomainExists.Should().BeFalse();
        }

        [Fact]
        public async Task WhenEmailDomainNotOrphaned_DoesNotDeletes()
        {
            var uniqueData = UNIQUE_PREFIX + "EDNOrph-NDel";
            var uniqueDomain = uniqueData + ".co";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var user1Id = await app.TestData.Users().AddAsync(uniqueData + 1, uniqueDomain);
            await app.TestData.Users().AddAsync(uniqueData + 2, uniqueDomain);

            var originalUser1 = await GetUserAsync(dbContext, user1Id);

            await contentRepository
                .Users()
                .DeleteAsync(user1Id);

            var emailDomainExists = await dbContext
                .EmailDomains
                .AsNoTracking()
                .AnyAsync(e => e.EmailDomainId == originalUser1.EmailDomainId);

            emailDomainExists.Should().BeTrue();
        }

        [Fact]
        public async Task WhenDeleted_ClearsAuthorizedTaskData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanDelete);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var taskCommand = await app.TestData.AuthorizedTasks().AddWithNewUserAsync(uniqueData, null, c => c.TaskData = uniqueData);

            await contentRepository
                .Users()
                .DeleteAsync(taskCommand.UserId);

            var authorizedTask = await dbContext
                .AuthorizedTasks
                .Where(t => t.AuthorizedTaskId == taskCommand.OutputAuthorizedTaskId)
                .SingleAsync();

            using (new AssertionScope())
            {
                authorizedTask.Should().NotBeNull();
                authorizedTask.TaskData.Should().BeNull();
            }
        }

        [Fact]
        public async Task WhenDeleted_RemovesUsernameFromAuthenticationFailLog()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanDelete);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var addUserCommand = app.TestData.Users().CreateAddCommand(uniqueData);
            var userId = await contentRepository.Users().AddAsync(addUserCommand);

            await contentRepository
                .Users()
                .Authentication()
                .AuthenticateCredentials(new AuthenticateUserCredentialsQuery()
                {
                    UserAreaCode = TestUserArea1.Code,
                    Password = "1234",
                    Username = addUserCommand.Email
                })
                .ExecuteAsync();

            await contentRepository
                .Users()
                .DeleteAsync(userId);

            var deletedUser = await GetUserAsync(dbContext, userId);
            var credentialLog = await dbContext
                .UserAuthenticationFailLogs
                .Where(u => u.Username == deletedUser.Username)
                .SingleOrDefaultAsync();

            credentialLog.Should().NotBeNull();
        }

        [Fact]
        public async Task WhenNotSuperAdmin_CannotDeleteSuperAdmin()
        {
            var uniqueData = UNIQUE_PREFIX + "SA_CantDelSA";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var userToDeleteId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(new AddUserCommand()
                {
                    Email = uniqueData + "@example.com",
                    Password = "DELETED!!!",
                    RoleCode = SuperAdminRole.Code,
                    UserAreaCode = CofoundryAdminUserArea.Code
                });

            var roleId = await app.TestData.Roles().AddAsync(uniqueData, CofoundryAdminUserArea.Code);
            var currentUserId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(new AddUserCommand()
                {
                    Email = uniqueData + ".current@example.com",
                    Password = "DELETED!!!",
                    RoleId = roleId,
                    UserAreaCode = CofoundryAdminUserArea.Code
                });

            await contentRepository
                .Users()
                .Authentication()
                .SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
                {
                    UserId = currentUserId
                });

            await contentRepository
                .Awaiting(r => r.WithContext<CofoundryAdminUserArea>().Users().DeleteAsync(userToDeleteId))
                .Should()
                .ThrowAsync<NotPermittedException>()
                .WithMessage($"* Super Administrator * manage other *");
        }

        [Fact]
        public async Task WhenRelatedDataDependency_Cascades()
        {
            var uniqueData = UNIQUE_PREFIX + "RelDep";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var userId = await app.TestData.Users().AddAsync(uniqueData);
            await app.TestData.UnstructuredData().AddAsync<UserEntityDefinition>(userId, RelatedEntityCascadeAction.Cascade);

            await contentRepository
                .Users()
                .DeleteAsync(userId);

            var dependency = await app.TestData.UnstructuredData().GetAsync<UserEntityDefinition>(userId);

            dependency.Should().BeNull();
        }

        [Fact]
        public async Task WhenDeleted_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "Del_SendMsg";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var userId = await app.TestData.Users().AddAsync(uniqueData);

            await contentRepository
                .Users()
                .DeleteAsync(userId);

            app.Mocks
                .CountMessagesPublished<UserDeletedMessage>(m =>
                {
                    return m.UserId == userId && m.UserAreaCode == TestUserArea1.Code;
                })
                .Should().Be(1);
        }

        private async Task<User> GetUserAsync(CofoundryDbContext dbContext, int userId)
        {
            return await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleOrDefaultAsync();
        }
    }
}
