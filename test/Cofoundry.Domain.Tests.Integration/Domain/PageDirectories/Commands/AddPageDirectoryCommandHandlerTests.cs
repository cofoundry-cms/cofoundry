using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Commands
{
    [Collection(nameof(DbDependentFixture))]
    public class AddPageDirectoryCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "AddPageDirectoryCHT ";
        private readonly TestDataHelper _testDataHelper;

        private readonly DbDependentFixture _dbDependentFixture;

        public AddPageDirectoryCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task WhenRootParent_Adds()
        {
            var directoryName = UNIQUE_PREFIX + nameof(WhenRootParent_Adds);
            var addDirectoryCommand = await _testDataHelper.PageDirectories().CreateAddCommandAsync(directoryName);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var directory = await dbContext
                .PageDirectories
                .AsNoTracking()
                .FilterByPageDirectoryId(addDirectoryCommand.OutputPageDirectoryId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                addDirectoryCommand.OutputPageDirectoryId.Should().BePositive();
                directory.Should().NotBeNull();
                directory.PageDirectoryId.Should().Be(addDirectoryCommand.OutputPageDirectoryId);
                directory.Name.Should().Be(directoryName);
                directory.UrlPath.Should().Be(SlugFormatter.ToSlug(directoryName));
                directory.ParentPageDirectoryId.Should().Be(addDirectoryCommand.ParentPageDirectoryId);
                directory.CreateDate.Should().NotBeDefault();
            }
        }

        [Fact]
        public async Task WhenNestedParent_Adds()
        {
            var directoryName = UNIQUE_PREFIX + nameof(WhenNestedParent_Adds);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(directoryName);

            var addChildDirectoryCommand = _testDataHelper
                .PageDirectories()
                .CreateAddCommand(directoryName + " Child", parentDirectoryId);

            await contentRepository
                .PageDirectories()
                .AddAsync(addChildDirectoryCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var childDirectory = await dbContext
                .PageDirectories
                .AsNoTracking()
                .Where(d => d.PageDirectoryId == addChildDirectoryCommand.OutputPageDirectoryId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                addChildDirectoryCommand.OutputPageDirectoryId.Should().BePositive();
                childDirectory.Should().NotBeNull();
                childDirectory.PageDirectoryId.Should().Be(addChildDirectoryCommand.OutputPageDirectoryId);
                childDirectory.ParentPageDirectoryId.Should().Be(parentDirectoryId);
            }
        }

        [Fact]
        public async Task WhenDeletedParent_Throws()
        {
            var directoryName = UNIQUE_PREFIX + nameof(WhenDeletedParent_Throws);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(directoryName);

            await contentRepository
                .PageDirectories()
                .DeleteAsync(parentDirectoryId);

            var addChildDirectoryCommand = _testDataHelper
                .PageDirectories()
                .CreateAddCommand(directoryName + " Child", parentDirectoryId);

            await contentRepository
                .Awaiting(r => r.PageDirectories().AddAsync(addChildDirectoryCommand))
                .Should()
                .ThrowAsync<EntityNotFoundException<PageDirectory>>()
                .WithId(parentDirectoryId);
        }

        [Fact]
        public async Task WhenDuplictate_Throws()
        {
            var directoryName = UNIQUE_PREFIX + nameof(WhenDuplictate_Throws);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(directoryName);
            var addDirectory2Command = await _testDataHelper.PageDirectories().CreateAddCommandAsync(directoryName);

            await contentRepository
                .Awaiting(r => r.PageDirectories().AddAsync(addDirectory2Command))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException>()
                .WithMemberNames(nameof(addDirectory2Command.UrlPath));
        }
    }
}
