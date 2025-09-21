﻿using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Users.Commands;

/// <summary>
/// Note: command defers most logic to DeleteUserCommandHandler.
/// </summary>
[Collection(nameof(IntegrationTestFixtureCollection))]
public class DeleteCurrentUserCommandHandlerTests
{
    const string UNIQUE_PREFIX = "DelCurUsrCHT-";
    private readonly IntegrationTestApplicationFactory _appFactory;

    public DeleteCurrentUserCommandHandlerTests(
        IntegrationTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task CanDelete()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanDelete);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var userId = await app.TestData.Users().AddAsync(uniqueData);
        var originalUser = await GetUserAsync(dbContext, userId);

        await contentRepository
            .Users()
            .Authentication()
            .SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
            {
                UserId = userId
            });

        await contentRepository
            .Users()
            .Current()
            .DeleteAsync();

        var deletedUser = await GetUserAsync(dbContext, userId);
        var signedInUser = await contentRepository
            .Users()
            .Current()
            .Get()
            .AsUserContext()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            deletedUser.Should().NotBeNull();
            deletedUser?.DeletedDate.Should().NotBeNull();
            signedInUser.IsSignedIn().Should().BeFalse();

            app.Mocks
                .CountMessagesPublished<UserDeletedMessage>(m =>
                {
                    return m.UserId == userId && m.UserAreaCode == TestUserArea1.Code;
                })
                .Should().Be(1);
        }
    }

    private static async Task<User?> GetUserAsync(CofoundryDbContext dbContext, int userId)
    {
        return await dbContext
            .Users
            .AsNoTracking()
            .FilterById(userId)
            .SingleOrDefaultAsync();
    }
}
