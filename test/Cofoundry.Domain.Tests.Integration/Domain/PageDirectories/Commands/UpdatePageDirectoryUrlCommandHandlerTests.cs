using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class UpdatePageDirectoryUrlCommandHandlerTests
{
    const string UNIQUE_PREFIX = "UpdPageDirectoryUrlCHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public UpdatePageDirectoryUrlCommandHandlerTests(
         DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task CanChangeUrl()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanChangeUrl);

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
            .UpdateUrlAsync(updateCommand);

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
            .Awaiting(r => r.PageDirectories().UpdateUrlAsync(updateCommand))
            .Should()
            .ThrowAsync<UniqueConstraintViolationException>()
            .WithMemberNames(nameof(updateCommand.UrlPath));
    }

    [Fact]
    public async Task WhenParentedToSelf_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenParentedToSelf_Throws);

        using var app = _appFactory.Create();
        var addDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .PageDirectories()
            .AddAsync(addDirectoryCommand);

        var updateCommand = MapFromAddCommand(addDirectoryCommand);
        updateCommand.ParentPageDirectoryId = updateCommand.PageDirectoryId;

        await contentRepository
            .Awaiting(r => r.PageDirectories().UpdateUrlAsync(updateCommand))
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMemberNames(nameof(updateCommand.ParentPageDirectoryId))
            .WithMessage("*cannot be parented to itself*");
    }

    [Fact]
    public async Task WhenParentedToChild_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenParentedToChild_Throws);

        using var app = _appFactory.Create();
        var addDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .PageDirectories()
            .AddAsync(addDirectoryCommand);
        var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData, addDirectoryCommand.OutputPageDirectoryId);
        var directory3Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory2Id);

        var updateCommand = MapFromAddCommand(addDirectoryCommand);
        updateCommand.ParentPageDirectoryId = directory3Id;

        await contentRepository
            .Awaiting(r => r.PageDirectories().UpdateUrlAsync(updateCommand))
            .Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMemberNames(nameof(updateCommand.ParentPageDirectoryId))
            .WithMessage("*parent directory cannot be * child*");
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
        var dir3Id = await app.TestData.PageDirectories().AddAsync("golf", dir2Id);
        var dir4Id = await app.TestData.PageDirectories().AddAsync("papa", dir3Id);

        await contentRepository
            .PageDirectories()
            .UpdateUrlAsync(new UpdatePageDirectoryUrlCommand()
            {
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
            .UpdateUrlAsync(new UpdatePageDirectoryUrlCommand()
            {
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

    [Fact]
    public async Task WhenUpdated_SendsMessages()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenUpdated_SendsMessages);
        var uniqueDataSlug = SlugFormatter.ToSlug(uniqueData);
        using var app = _appFactory.Create();
        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var directory2Id = await app.TestData.PageDirectories().AddAsync("2", directory1Id);
        var directory3Id = await app.TestData.PageDirectories().AddAsync("3", directory2Id);
        var directory1PageId = await app.TestData.Pages().AddAsync("d1p1", directory1Id, c => c.Publish = true);
        var directory3Page1Id = await app.TestData.Pages().AddAsync("d3p1", directory3Id, c => c.Publish = true);
        var directory3Page2Id = await app.TestData.Pages().AddAsync("d3p2", directory3Id, c => c.Publish = false);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .PageDirectories()
            .UpdateUrlAsync(new UpdatePageDirectoryUrlCommand()
            {
                PageDirectoryId = directory1Id,
                ParentPageDirectoryId = app.SeededEntities.RootDirectoryId,
                UrlPath = uniqueDataSlug + "-changed"
            });

        var directoryMessages = new Dictionary<int, string>()
        {
            { directory1Id, $"/{uniqueDataSlug}" },
            { directory2Id, $"/{uniqueDataSlug}/2" },
            { directory3Id, $"/{uniqueDataSlug}/2/3" },
        };

        var pageMessages = new Dictionary<int, string>()
        {
            { directory1PageId, $"/{uniqueDataSlug}/d1p1" },
            { directory3Page1Id, $"/{uniqueDataSlug}/2/3/d3p1" },
            { directory3Page2Id, $"/{uniqueDataSlug}/2/3/d3p2" },
        };

        using (new AssertionScope())
        {
            foreach (var message in pageMessages)
            {
                app.Mocks
                    .CountMessagesPublished<PageUrlChangedMessage>(m =>
                    {
                        return m.PageId == message.Key
                            && m.OldFullUrlPath == message.Value
                            && m.HasPublishedVersionChanged == (message.Key != directory3Page2Id);
                    })
                    .Should().Be(1);
            }
            foreach (var message in directoryMessages)
            {
                app.Mocks
                    .CountMessagesPublished<PageDirectoryUrlChangedMessage>(m =>
                    {
                        return m.PageDirectoryId == message.Key && m.OldFullUrlPath == message.Value;
                    })
                    .Should().Be(1);
            }
        }
    }

    public UpdatePageDirectoryUrlCommand MapFromAddCommand(AddPageDirectoryCommand command)
    {
        return new UpdatePageDirectoryUrlCommand()
        {
            PageDirectoryId = command.OutputPageDirectoryId,
            ParentPageDirectoryId = command.ParentPageDirectoryId,
            UrlPath = command.UrlPath
        };
    }
}
