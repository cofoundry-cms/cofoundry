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

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class UpdatePageDirectoryCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "UpdPageDirectoryCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public UpdatePageDirectoryCommandHandlerTests(
             DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanUpdateProperties()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanUpdateProperties);

            using var app = _appFactory.Create();
            var addDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.Name = uniqueData + "U";
            await contentRepository
                .PageDirectories()
                .UpdateAsync(updateCommand);

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directory = await dbContext
                .PageDirectories
                .AsNoTracking()
                .FilterById(addDirectoryCommand.OutputPageDirectoryId)
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
            var uniqueData = UNIQUE_PREFIX + nameof(WhenNotInUse_CanChangeUrl);

            using var app = _appFactory.Create();
            var addDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);
            var newParentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData + "2");

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.UrlPath = addDirectoryCommand.UrlPath + "U";
            updateCommand.ParentPageDirectoryId = newParentDirectoryId;

            await contentRepository
                .PageDirectories()
                .UpdateAsync(updateCommand);

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directory = await dbContext
                .PageDirectories
                .AsNoTracking()
                .FilterById(addDirectoryCommand.OutputPageDirectoryId)
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
            var uniqueData = UNIQUE_PREFIX + nameof(WhenDirectoryInUseAndChangingUrl_Throws);

            using var app = _appFactory.Create();
            var addDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var childDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData, addDirectoryCommand.OutputPageDirectoryId);
            await app.TestData.Pages().AddAsync(uniqueData, childDirectoryId);

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
            var uniqueData = UNIQUE_PREFIX + "WhenDirInUseAndChangingParentDir";

            using var app = _appFactory.Create();
            var addDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var childDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData, addDirectoryCommand.OutputPageDirectoryId);
            await app.TestData.Pages().AddAsync(uniqueData, childDirectoryId);
            var newParentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");

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
            var uniqueData = UNIQUE_PREFIX + nameof(WhenNotUnique_Throws);

            using var app = _appFactory.Create();
            var addDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);
            var duplicatePath = addDirectoryCommand.UrlPath + "1";

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            await app.TestData.PageDirectories().AddAsync(duplicatePath);

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
