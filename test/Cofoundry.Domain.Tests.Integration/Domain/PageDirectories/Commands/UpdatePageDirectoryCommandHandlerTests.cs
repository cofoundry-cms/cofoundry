using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration
{
    [Collection(nameof(DbDependentFixture))]
    public class UpdatePageDirectoryCommandHandlerTests
    {
        const string DIRECTORY_PREFIX = "UpdPageDirectoryCHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public UpdatePageDirectoryCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task CanUpdateProperties()
        {
            var uniqueData = DIRECTORY_PREFIX + nameof(CanUpdateProperties);
            var addDirectoryCommand = await _testDataHelper.PageDirectories.CreateAddCommandAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.Name = uniqueData + "U";
            await contentRepository
                .PageDirectories()
                .UpdateAsync(updateCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var directory = await dbContext
                .PageDirectories
                .AsNoTracking()
                .FilterByPageDirectoryId(addDirectoryCommand.OutputPageDirectoryId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                directory.Should().NotBeNull();
                directory.Name.Should().Be(updateCommand.Name);
                directory.UrlPath.Should().Be(addDirectoryCommand.UrlPath);
                directory.ParentPageDirectoryId.Should().Be(addDirectoryCommand.ParentPageDirectoryId);
            }
        }

        [Fact]
        public async Task WhenNotInUse_CanChangeUrl()
        {
            var uniqueData = DIRECTORY_PREFIX + nameof(WhenNotInUse_CanChangeUrl);
            var addDirectoryCommand = await _testDataHelper.PageDirectories.CreateAddCommandAsync(uniqueData);
            var newParentDirectoryId = await _testDataHelper.PageDirectories.AddAsync(uniqueData + "2");

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.UrlPath = addDirectoryCommand.UrlPath + "U";
            updateCommand.ParentPageDirectoryId = newParentDirectoryId;

            await contentRepository
                .PageDirectories()
                .UpdateAsync(updateCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var directory = await dbContext
                .PageDirectories
                .AsNoTracking()
                .FilterByPageDirectoryId(addDirectoryCommand.OutputPageDirectoryId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                directory.UrlPath.Should().Be(updateCommand.UrlPath);
                directory.ParentPageDirectoryId.Should().Be(updateCommand.ParentPageDirectoryId);
            }
        }

        [Fact]
        public async Task WhenDirectoryInUseAndChangingUrl_Throws()
        {
            var uniqueData = DIRECTORY_PREFIX + nameof(WhenDirectoryInUseAndChangingUrl_Throws);
            var addDirectoryCommand = await _testDataHelper.PageDirectories.CreateAddCommandAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var childDirectoryId = await _testDataHelper.PageDirectories.AddAsync(uniqueData, addDirectoryCommand.OutputPageDirectoryId);
            await _testDataHelper.Pages.AddAsync(uniqueData, childDirectoryId);

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.UrlPath = uniqueData + "U";

            await contentRepository
                .Awaiting(r => r.PageDirectories().UpdateAsync(updateCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(updateCommand.UrlPath));
        }

        [Fact]
        public async Task WhenDirectoryInUseAndChangingParentDirectory_Throws()
        {
            var uniqueData = DIRECTORY_PREFIX + "WhenDirInUseAndChangingParentDir";
            var addDirectoryCommand = await _testDataHelper.PageDirectories.CreateAddCommandAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var childDirectoryId = await _testDataHelper.PageDirectories.AddAsync(uniqueData, addDirectoryCommand.OutputPageDirectoryId);
            await _testDataHelper.Pages.AddAsync(uniqueData, childDirectoryId);
            var newParentDirectoryId = await _testDataHelper.PageDirectories.AddAsync(uniqueData + "1");

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.ParentPageDirectoryId = newParentDirectoryId;

            await contentRepository
                .Awaiting(r => r.PageDirectories().UpdateAsync(updateCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(updateCommand.ParentPageDirectoryId));
        }

        [Fact]
        public async Task WhenNotUnique_Throws()
        {
            var uniqueData = DIRECTORY_PREFIX + nameof(WhenNotUnique_Throws);

            var addDirectoryCommand = await _testDataHelper.PageDirectories.CreateAddCommandAsync(uniqueData);
            var duplicatePath = addDirectoryCommand.UrlPath + "1";

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            await _testDataHelper.PageDirectories.AddAsync(duplicatePath);

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.UrlPath = duplicatePath;

            await contentRepository
                .Awaiting(r => r.PageDirectories().UpdateAsync(updateCommand))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException>()
                .WithMemberNames(nameof(updateCommand.UrlPath));
        }


        public UpdatePageDirectoryCommand MapFromAddCommand(AddPageDirectoryCommand command)
        {
            return new UpdatePageDirectoryCommand()
            {
                Name = command.Name,
                PageDirectoryId = command.OutputPageDirectoryId,
                ParentPageDirectoryId = command.ParentPageDirectoryId,
                UrlPath = command.UrlPath
            };
        }
    }
}
