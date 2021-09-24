using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Commands
{
    [Collection(nameof(DbDependentFixture))]
    public class DeletePageDirectoryCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DelPageDirectoryCHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public DeletePageDirectoryCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task WhenRootParent_DoesNotDeleteSiblings()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenRootParent_DoesNotDeleteSiblings);
            var pageDirectory1Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + "1");
            var pageDirectory2Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + "2");

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .DeleteAsync(pageDirectory1Id);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var directory1Exists = await DoesDirectoryExistsAsync(pageDirectory1Id);
            var directory2Exists = await DoesDirectoryExistsAsync(pageDirectory2Id);

            using (new AssertionScope())
            {
                directory1Exists.Should().BeFalse();
                directory2Exists.Should().BeTrue();
            }
        }

        [Fact]
        public async Task WhenHasChildren_DeletesChildren()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenHasChildren_DeletesChildren);

            var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData + " P");
            var childDirectory1Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + " C1", parentDirectoryId);
            var childDirectory2Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + " C2", childDirectory1Id);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, childDirectory2Id);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .DeleteAsync(parentDirectoryId);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var parentDirectoryExists = await DoesDirectoryExistsAsync(parentDirectoryId);
            var childDirectory1Exists = await DoesDirectoryExistsAsync(childDirectory1Id);
            var childDirectory2Exists = await DoesDirectoryExistsAsync(childDirectory2Id);
            var pageExists = await dbContext
                .Pages
                .AsNoTracking()
                .FilterByPageId(pageId)
                .AnyAsync();

            using (new AssertionScope())
            {
                parentDirectoryExists.Should().BeFalse();
                childDirectory1Exists.Should().BeFalse();
                childDirectory2Exists.Should().BeFalse();
                pageExists.Should().BeFalse();
            }
        }

        [Fact]
        public async Task WhenRoot_Throws()
        {
            var rootDirectoryId = await _testDataHelper.PageDirectories().GetRootDirectoryIdAsync();

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .Awaiting(r => r.PageDirectories().DeleteAsync(rootDirectoryId))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames("PageDirectoryId");
        }

        private async Task<bool> DoesDirectoryExistsAsync(int directoryId)
        {
            using var scope = _dbDependentFixture.CreateServiceScope();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var exists = await dbContext
                .PageDirectories
                .AsNoTracking()
                .FilterByPageDirectoryId(directoryId)
                .AnyAsync();

            return exists;
        }
    }
}
