﻿using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.PageDirectories.Commands;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class UpdatePageDirectoryCommandHandlerTests
{
    const string UNIQUE_PREFIX = "UpdPageDirectoryCHT ";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public UpdatePageDirectoryCommandHandlerTests(
         IntegrationTestApplicationFactory appFactory
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
            directory?.Name.Should().Be(updateCommand.Name);
            directory?.UrlPath.Should().Be(addDirectoryCommand.UrlPath);
            directory?.ParentPageDirectoryId.Should().Be(addDirectoryCommand.ParentPageDirectoryId);
        }
    }

    private static UpdatePageDirectoryCommand MapFromAddCommand(AddPageDirectoryCommand command)
    {
        return new UpdatePageDirectoryCommand()
        {
            PageDirectoryId = command.OutputPageDirectoryId,
            Name = command.Name
        };
    }
}
