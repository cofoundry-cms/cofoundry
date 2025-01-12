﻿using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.AuthorizedTasks.Commands;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class CleanupAuthorizedTasksCommandHandlerTests
{
    const string UNIQUE_PREFIX = "CUpAuthTskCHT-";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public CleanupAuthorizedTasksCommandHandlerTests(
         IntegrationTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task WhenComplete_Deletes()
    {
        var uniqueData = UNIQUE_PREFIX + "Comp_Del";
        var seedDate = new DateTime(1994, 4, 11);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        app.Mocks.MockDateTime(seedDate);

        var addAuthorizedTaskCommand = await app
            .TestData
            .AuthorizedTasks()
            .AddWithNewUserAsync(uniqueData, seedDate);

        await contentRepository
            .AuthorizedTasks()
            .CompleteAsync(new CompleteAuthorizedTaskCommand(addAuthorizedTaskCommand.OutputAuthorizedTaskId));

        app.Mocks.MockDateTime(seedDate.AddDays(31));

        await contentRepository
            .ExecuteCommandAsync(new CleanupAuthorizedTasksCommand()
            {
                RetentionPeriod = TimeSpan.FromDays(30)
            });

        var authorizedTask = await dbContext
            .AuthorizedTasks
            .AsNoTracking()
            .Where(t => t.AuthorizedTaskId == addAuthorizedTaskCommand.OutputAuthorizedTaskId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            authorizedTask.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenNotComplete_DoesNotDelete()
    {
        var uniqueData = UNIQUE_PREFIX + "NotComp_NotDel";
        var seedDate = new DateTime(1994, 6, 20);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        app.Mocks.MockDateTime(seedDate);

        var addAuthorizedTaskCommand = await app
            .TestData
            .AuthorizedTasks()
            .AddWithNewUserAsync(uniqueData, seedDate);

        app.Mocks.MockDateTime(seedDate.AddYears(1));

        await contentRepository
            .ExecuteCommandAsync(new CleanupAuthorizedTasksCommand()
            {
                RetentionPeriod = TimeSpan.FromDays(30)
            });

        var authorizedTask = await dbContext
            .AuthorizedTasks
            .AsNoTracking()
            .Where(t => t.AuthorizedTaskId == addAuthorizedTaskCommand.OutputAuthorizedTaskId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            authorizedTask.Should().NotBeNull();
            authorizedTask?.CompletedDate.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenInvalidated_Deletes()
    {
        var uniqueData = UNIQUE_PREFIX + "Inv_Del";
        var seedDate = new DateTime(1994, 8, 8);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        app.Mocks.MockDateTime(seedDate);

        var addAuthorizedTaskCommand = await app
            .TestData
            .AuthorizedTasks()
            .AddWithNewUserAsync(uniqueData, seedDate);

        await contentRepository
            .AuthorizedTasks()
            .InvalidateBatchAsync(new InvalidateAuthorizedTaskBatchCommand(addAuthorizedTaskCommand.UserId));

        app.Mocks.MockDateTime(seedDate.AddDays(31));

        await contentRepository
            .ExecuteCommandAsync(new CleanupAuthorizedTasksCommand()
            {
                RetentionPeriod = TimeSpan.FromDays(30)
            });

        var authorizedTask = await dbContext
            .AuthorizedTasks
            .AsNoTracking()
            .Where(t => t.AuthorizedTaskId == addAuthorizedTaskCommand.OutputAuthorizedTaskId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            authorizedTask.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenExpired_Deletes()
    {
        var uniqueData = UNIQUE_PREFIX + "Exp_Del";
        var seedDate = new DateTime(1994, 10, 10);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        app.Mocks.MockDateTime(seedDate);

        var addAuthorizedTaskCommand = await app
            .TestData
            .AuthorizedTasks()
            .AddWithNewUserAsync(uniqueData, seedDate, c => c.ExpireAfter = TimeSpan.FromDays(7));

        await contentRepository
            .AuthorizedTasks()
            .InvalidateBatchAsync(new InvalidateAuthorizedTaskBatchCommand(addAuthorizedTaskCommand.UserId));

        app.Mocks.MockDateTime(seedDate.AddDays(38));

        await contentRepository
            .ExecuteCommandAsync(new CleanupAuthorizedTasksCommand()
            {
                RetentionPeriod = TimeSpan.FromDays(30)
            });

        var authorizedTask = await dbContext
            .AuthorizedTasks
            .AsNoTracking()
            .Where(t => t.AuthorizedTaskId == addAuthorizedTaskCommand.OutputAuthorizedTaskId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            authorizedTask.Should().BeNull();
        }
    }
}
