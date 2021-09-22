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

namespace Cofoundry.Domain.Tests.Integration
{
    [Collection(nameof(DbDependentFixture))]
    public class AddPageDirectoryCommandHandlerTests
    {
        const string DIRECTORY_PREFIX = "AddPageDirectoryCHT ";

        private readonly DbDependentFixture _dbDependentFixture;

        public AddPageDirectoryCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
        }

        [Fact]
        public async Task WhenRootParent_Adds()
        {
            var directoryName = DIRECTORY_PREFIX + nameof(WhenRootParent_Adds);
            var addDirectoryCommand = await CreateValidCommandWithRootParentDirectoryAsync(directoryName, _dbDependentFixture);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var directory = await dbContext
                .PageDirectories
                .Where(d => d.PageDirectoryId == addDirectoryCommand.OutputPageDirectoryId)
                .AsNoTracking()
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
            var directoryName = DIRECTORY_PREFIX + nameof(WhenNestedParent_Adds);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var addParentDirectoryCommand = await CreateValidCommandWithRootParentDirectoryAsync(directoryName, _dbDependentFixture);
            await contentRepository
                .PageDirectories()
                .AddAsync(addParentDirectoryCommand);

            var addChildDirectoryCommand = CreateValidCommand(directoryName + " Child", addParentDirectoryCommand.OutputPageDirectoryId);
            await contentRepository
                .WithElevatedPermissions()
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
                childDirectory.ParentPageDirectoryId.Should().Be(addChildDirectoryCommand.ParentPageDirectoryId);
            }
        }

        [Fact]
        public async Task WhenDeletedParent_Throws()
        {
            var directoryName = DIRECTORY_PREFIX + nameof(WhenDeletedParent_Throws);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var addParentDirectoryCommand = await CreateValidCommandWithRootParentDirectoryAsync(directoryName, _dbDependentFixture);
            await contentRepository
                .PageDirectories()
                .AddAsync(addParentDirectoryCommand);

            await contentRepository
                .PageDirectories()
                .DeleteAsync(addParentDirectoryCommand.OutputPageDirectoryId);

            var addChildDirectoryCommand = CreateValidCommand(directoryName + " Child", addParentDirectoryCommand.OutputPageDirectoryId);

            await contentRepository
                .Awaiting(r => r.PageDirectories().AddAsync(addChildDirectoryCommand))
                .Should()
                .ThrowAsync<EntityNotFoundException<PageDirectory>>()
                .WithId(addChildDirectoryCommand.ParentPageDirectoryId);
        }

        [Fact]
        public async Task WhenDuplictate_Throws()
        {
            var directoryName = DIRECTORY_PREFIX + nameof(WhenDuplictate_Throws);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var addDirectory1Command = await CreateValidCommandWithRootParentDirectoryAsync(directoryName, _dbDependentFixture);
            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectory1Command);

            var addDirectory2Command = await CreateValidCommandWithRootParentDirectoryAsync(directoryName, _dbDependentFixture);

            await contentRepository
                .Awaiting(r => r.PageDirectories().AddAsync(addDirectory2Command))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException>()
                .WithMemberNames(nameof(addDirectory2Command.UrlPath));
        }

        private static async Task<int> GetRootDirectoryIdAsync(DbDependentFixture dbDependentFixture)
        {
            using var scope = dbDependentFixture.CreateServiceScope();
            using var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            return await dbContext
                .PageDirectories
                .Where(d => !d.ParentPageDirectoryId.HasValue)
                .Select(d => d.PageDirectoryId)
                .SingleAsync();
        }

        public static async Task<AddPageDirectoryCommand> CreateValidCommandWithRootParentDirectoryAsync(string uniqueData, DbDependentFixture dbDependentFixture)
        {
            var rootDirectoryId = await GetRootDirectoryIdAsync(dbDependentFixture);
            return CreateValidCommand(uniqueData, rootDirectoryId);
        }

        public static AddPageDirectoryCommand CreateValidCommand(string uniqueData, int parentDirectoryId)
        {
            var command = new AddPageDirectoryCommand()
            {
                Name = uniqueData,
                ParentPageDirectoryId = parentDirectoryId,
                UrlPath = SlugFormatter.ToSlug(uniqueData)
            };

            return command;
        }
    }
}
