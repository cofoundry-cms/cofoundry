using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            await app.TestData.Pages().AddAsync(uniqueData, childDirectoryId, c => c.Publish = true);

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.UrlPath = updateCommand.UrlPath + "u";

            await contentRepository
                .Awaiting(r => r.PageDirectories().UpdateAsync(updateCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("*directory * in use*")
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
            await app.TestData.Pages().AddAsync(uniqueData, childDirectoryId, c => c.Publish = true);
            var newParentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");

            var updateCommand = MapFromAddCommand(addDirectoryCommand);
            updateCommand.ParentPageDirectoryId = newParentDirectoryId;

            await contentRepository
                .Awaiting(r => r.PageDirectories().UpdateAsync(updateCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("*directory * in use*")
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

        [Fact]
        public async Task UpdatingParentDirectory_UpdatesPageDirectoryClosureTable()
        {
            var uniqueData = UNIQUE_PREFIX + "UpdUrl_UpdPDClosure";
            var sluggedPath = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var dir1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var dir2Id = await app.TestData.PageDirectories().AddAsync("alpha", dir1Id);
            var dir3Id = await app.TestData.PageDirectories().AddAsync("bravo", dir2Id);
            var dir4Id = await app.TestData.PageDirectories().AddAsync("papa", dir3Id);

            await contentRepository
                .PageDirectories()
                .UpdateAsync(new UpdatePageDirectoryCommand()
                {
                    Name = "Papa",
                    UrlPath = "papa",
                    PageDirectoryId = dir4Id,
                    ParentPageDirectoryId = dir2Id
                });

            var directory4Closures = await dbContext
                .PageDirectoryClosures
                .AsNoTracking()
                .FilterByDescendantId(dir4Id)
                .ToListAsync();

            using (new AssertionScope())
            {
                directory4Closures.Should().HaveCount(4);

                var selfRefNode = directory4Closures.FilterSelfReferencing().SingleOrDefault();
                selfRefNode.Should().NotBeNull();
                selfRefNode.Distance.Should().Be(0);

                var dir2Ancestor = directory4Closures.FilterByAncestorId(dir2Id).SingleOrDefault();
                dir2Ancestor.Should().NotBeNull();
                dir2Ancestor.Distance.Should().Be(1);

                var dir1Ancestor = directory4Closures.FilterByAncestorId(dir1Id).SingleOrDefault();
                dir1Ancestor.Should().NotBeNull();
                dir1Ancestor.Distance.Should().Be(2);

                var rootDirectoryAncestor = directory4Closures.FilterByAncestorId(app.SeededEntities.RootDirectoryId).SingleOrDefault();
                rootDirectoryAncestor.Should().NotBeNull();
                rootDirectoryAncestor.Distance.Should().Be(3);
            }
        }

        [Fact]
        public async Task UpdatingUrl_UpdatesPageDirectoryPathTable()
        {
            var uniqueData = UNIQUE_PREFIX + "UpdUrl_UpdPDPath";
            var sluggedPath = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var dir1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var dir2Id = await app.TestData.PageDirectories().AddAsync("delta", dir1Id);
            var dir3Id = await app.TestData.PageDirectories().AddAsync("india", dir2Id);
            var dir4Id = await app.TestData.PageDirectories().AddAsync("golf", dir3Id);

            await contentRepository
                .PageDirectories()
                .UpdateAsync(new UpdatePageDirectoryCommand()
                {
                    Name = "Uniform",
                    UrlPath = "uniform",
                    PageDirectoryId = dir3Id,
                    ParentPageDirectoryId = dir2Id
                });

            var directory3Path = await dbContext
                .PageDirectoryPaths
                .AsNoTracking()
                .Where(d => d.PageDirectoryId == dir3Id)
                .SingleOrDefaultAsync();

            var directory4Path = await dbContext
                .PageDirectoryPaths
                .AsNoTracking()
                .Where(d => d.PageDirectoryId == dir4Id)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                directory3Path.Should().NotBeNull();
                directory3Path.Depth.Should().Be(3);
                directory3Path.FullUrlPath.Should().Be($"{sluggedPath}/delta/uniform");

                directory4Path.Should().NotBeNull();
                directory4Path.Depth.Should().Be(4);
                directory4Path.FullUrlPath.Should().Be($"{sluggedPath}/delta/uniform/golf");
            }
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
