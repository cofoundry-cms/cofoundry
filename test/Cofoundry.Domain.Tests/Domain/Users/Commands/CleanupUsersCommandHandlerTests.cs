﻿using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.AuthorizedTasks.Commands;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class CleanupUsersCommandHandlerTests
{
    const string UNIQUE_PREFIX = "CUpUsrCHT-";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public CleanupUsersCommandHandlerTests(
         IntegrationTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task CanClearStaleAuthenticationData()
    {
        var uniqueData = UNIQUE_PREFIX + "CCStaleAuth";
        var seedDate = new DateTime(1994, 8, 2, 0, 0, 0, DateTimeKind.Utc);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var userId = await app.TestData.Users().AddAsync(uniqueData);
        var logCommand = new LogSuccessfulAuthenticationCommand()
        {
            UserId = userId
        };

        app.Mocks.MockDateTime(seedDate);
        await contentRepository.ExecuteCommandAsync(logCommand);
        app.Mocks.MockDateTime(seedDate.AddDays(50));
        await contentRepository.ExecuteCommandAsync(logCommand);

        app.Mocks.MockDateTime(seedDate.AddDays(70));
        await contentRepository
            .ExecuteCommandAsync(new CleanupUsersCommand()
            {
                UserAreaCode = TestUserArea1.Code,
                DefaultRetentionPeriod = TimeSpan.FromDays(30)
            });

        var logs = await dbContext
            .UserAuthenticationLogs
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .ToArrayAsync();

        using (new AssertionScope())
        {
            logs.Should().HaveCount(1);
            logs.Single().CreateDate.Should().Be(seedDate.AddDays(50));
        }
    }

    [Fact]
    public async Task CanClearStaleAuthenticationFailData()
    {
        var uniqueData = UNIQUE_PREFIX + "CCStaleAuthF";
        var seedDate = new DateTime(1994, 8, 2, 0, 0, 0, DateTimeKind.Utc);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var logCommand = new LogFailedAuthenticationAttemptCommand()
        {
            UserAreaCode = TestUserArea1.Code,
            Username = uniqueData
        };

        app.Mocks.MockDateTime(seedDate);
        await contentRepository.ExecuteCommandAsync(logCommand);
        app.Mocks.MockDateTime(seedDate.AddDays(50));
        await contentRepository.ExecuteCommandAsync(logCommand);

        app.Mocks.MockDateTime(seedDate.AddDays(70));
        await contentRepository
            .ExecuteCommandAsync(new CleanupUsersCommand()
            {
                UserAreaCode = TestUserArea1.Code,
                DefaultRetentionPeriod = TimeSpan.FromDays(30)
            });

        var logs = await dbContext
            .UserAuthenticationFailLogs
            .AsNoTracking()
            .Where(u => u.Username == uniqueData)
            .ToListAsync();

        using (new AssertionScope())
        {
            logs.Should().HaveCount(1);
            logs.Single().CreateDate.Should().Be(seedDate.AddDays(50));
        }
    }
}
