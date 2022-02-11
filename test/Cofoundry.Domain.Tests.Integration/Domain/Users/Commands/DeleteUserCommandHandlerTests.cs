using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class DeleteUserCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DelUserCHT ";
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
            var userId = await app.TestData.Users().AddAsync(uniqueData);

            await contentRepository
                .Users()
                .DeleteAsync(userId);

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var user = await dbContext
                .Users
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.UserId == userId);

            user.Should().NotBeNull();
            user.IsDeleted.Should().BeTrue();
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
        public async Task WhenRequiredRelatedDataDependency_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "ReqRelDep";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var userId = await app.TestData.Users().AddAsync(uniqueData);
            await app.TestData.UnstructuredData().AddAsync<UserEntityDefinition>(userId, RelatedEntityCascadeAction.None);

            await contentRepository
                .Awaiting(r => r.Users().DeleteAsync(userId))
                .Should()
                .ThrowAsync<RequiredDependencyConstaintViolationException>()
                .WithMessage($"Cannot delete * User * {TestCustomEntityDefinition.EntityName} '{app.SeededEntities.CustomEntityForUnstructuredDataTests.Title}' * dependency*");
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
    }
}
