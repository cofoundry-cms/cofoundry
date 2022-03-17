using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddPageDirectoryCommandHandlerTests
{
    const string UNIQUE_PREFIX = "AddPageDirectoryCHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public AddPageDirectoryCommandHandlerTests(
         DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task WhenRootParent_Adds()
    {
        var directoryName = UNIQUE_PREFIX + nameof(WhenRootParent_Adds);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var addDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(directoryName);
        await contentRepository
            .PageDirectories()
            .AddAsync(addDirectoryCommand);

        var directory = await dbContext
            .PageDirectories
            .AsNoTracking()
            .FilterById(addDirectoryCommand.OutputPageDirectoryId)
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

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(directoryName);
        var addChildDirectoryCommand = app.TestData.PageDirectories().CreateAddCommand(directoryName + " Child", parentDirectoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .PageDirectories()
            .AddAsync(addChildDirectoryCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

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

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(directoryName);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .PageDirectories()
            .DeleteAsync(parentDirectoryId);

        var addChildDirectoryCommand = app.TestData
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

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(directoryName);
        var addDirectory2Command = await app.TestData.PageDirectories().CreateAddCommandAsync(directoryName);

        await contentRepository
            .Awaiting(r => r.PageDirectories().AddAsync(addDirectory2Command))
            .Should()
            .ThrowAsync<UniqueConstraintViolationException>()
            .WithMemberNames(nameof(addDirectory2Command.UrlPath));
    }

    [Fact]
    public async Task WhenAdded_UpdatesPageDirectoryClosureTable()
    {
        var directoryName = UNIQUE_PREFIX + "UpdClosureTable";

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(directoryName);
        var addChildDirectoryCommand = app.TestData.PageDirectories().CreateAddCommand(directoryName + " Child", parentDirectoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .PageDirectories()
            .AddAsync(addChildDirectoryCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var directoryClosures = await dbContext
            .PageDirectoryClosures
            .AsNoTracking()
            .FilterByDescendantId(addChildDirectoryCommand.OutputPageDirectoryId)
            .ToListAsync();

        using (new AssertionScope())
        {
            directoryClosures.Should().HaveCount(3);

            var selfRefNode = directoryClosures.FilterSelfReferencing().SingleOrDefault();
            selfRefNode.Should().NotBeNull();
            selfRefNode.Distance.Should().Be(0);

            var parentDirectoryAncestor = directoryClosures.FilterByAncestorId(parentDirectoryId).SingleOrDefault();
            parentDirectoryAncestor.Should().NotBeNull();
            parentDirectoryAncestor.Distance.Should().Be(1);

            var rootDirectoryAncestor = directoryClosures.FilterByAncestorId(app.SeededEntities.RootDirectoryId).SingleOrDefault();
            rootDirectoryAncestor.Should().NotBeNull();
            rootDirectoryAncestor.Distance.Should().Be(2);
        }
    }

    [Fact]
    public async Task WhenAdded_UpdatesPageDirectoryPathTable()
    {
        var directoryName = UNIQUE_PREFIX + "UpdPathTable";
        var sluggedPath = SlugFormatter.ToSlug(directoryName);

        using var app = _appFactory.Create();
        var dir1Id = await app.TestData.PageDirectories().AddAsync(directoryName);
        var dir2Id = await app.TestData.PageDirectories().AddAsync("red", dir1Id);
        var dir3Id = await app.TestData.PageDirectories().AddAsync("blue", dir2Id);
        var dir4Id = await app.TestData.PageDirectories().AddAsync("green", dir3Id);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var directoryPath = await dbContext
            .PageDirectoryPaths
            .AsNoTracking()
            .Where(d => d.PageDirectoryId == dir4Id)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            directoryPath.Should().NotBeNull();
            directoryPath.Depth.Should().Be(4);
            directoryPath.FullUrlPath.Should().Be($"{sluggedPath}/red/blue/green");
        }
    }

    [Fact]
    public async Task WhenAdded_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenAdded_SendsMessage);

        using var app = _appFactory.Create();
        var command = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .PageDirectories()
            .AddAsync(command);

        app.Mocks
            .CountMessagesPublished<PageDirectoryAddedMessage>(m => m.PageDirectoryId == command.OutputPageDirectoryId)
            .Should().Be(1);
    }
}
