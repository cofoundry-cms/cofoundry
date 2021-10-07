using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class IsPageAccessRuleUniqueQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "IsPageAccRuleUnqQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public IsPageAccessRuleUniqueQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        public async Task WhenUserAreaUnique_ReturnsTrue()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUserAreaUnique_ReturnsTrue);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            await contentRepository
                .Pages()
                .AccessRules()
                .AddAsync(new AddPageAccessRuleCommand()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                });

            var isUnique = await contentRepository
                .Pages()
                .AccessRules()
                .IsUnique(new IsPageAccessRuleUniqueQuery()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode
                })
                .ExecuteAsync();

            isUnique.Should().BeTrue();
        }

        public async Task WhenUserAreaNotUnique_ReturnsFalse()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUserAreaNotUnique_ReturnsFalse);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            await contentRepository
                .Pages()
                .AccessRules()
                .AddAsync(new AddPageAccessRuleCommand()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                });

            var isUnique = await contentRepository
                .Pages()
                .AccessRules()
                .IsUnique(new IsPageAccessRuleUniqueQuery()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                })
                .ExecuteAsync();

            isUnique.Should().BeFalse();
        }

        public async Task WhenRoleUnique_ReturnsTrue()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUserAreaUnique_ReturnsTrue);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            await contentRepository
                .Pages()
                .AccessRules()
                .AddAsync(new AddPageAccessRuleCommand()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleId
                });

            await contentRepository
                .Pages()
                .AccessRules()
                .AddAsync(new AddPageAccessRuleCommand()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                });

            var isUnique = await contentRepository
                .Pages()
                .AccessRules()
                .IsUnique(new IsPageAccessRuleUniqueQuery()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea2.RoleId
                })
                .ExecuteAsync();

            isUnique.Should().BeTrue();
        }

        public async Task WhenRoleNotUnique_ReturnsFalse()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUserAreaNotUnique_ReturnsFalse);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            await contentRepository
                .Pages()
                .AccessRules()
                .AddAsync(new AddPageAccessRuleCommand()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleId
                });

            var isUnique = await contentRepository
                .Pages()
                .AccessRules()
                .IsUnique(new IsPageAccessRuleUniqueQuery()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleId
                })
                .ExecuteAsync();

            isUnique.Should().BeFalse();
        }

        public async Task WhenExistingRule_ReturnsFalse()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenExistingRule_ReturnsFalse);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var accessRuleId = await contentRepository
                .Pages()
                .AccessRules()
                .AddAsync(new AddPageAccessRuleCommand()
                {
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleId
                });

            var isUnique = await contentRepository
                .Pages()
                .AccessRules()
                .IsUnique(new IsPageAccessRuleUniqueQuery()
                {
                    PageAccessRuleId = accessRuleId,
                    PageId = pageId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleId
                })
                .ExecuteAsync();

            isUnique.Should().BeFalse();
        }
    }
}
