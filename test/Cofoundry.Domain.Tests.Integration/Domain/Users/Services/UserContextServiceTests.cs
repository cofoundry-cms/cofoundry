﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Tests.Integration.Users.Services;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class UserContextServiceTests
{
    const string UNIQUE_PREFIX = "UserContextSvc";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public UserContextServiceTests(
        IntegrationTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task GetCurrentContextAsync_WhenNotSignedIn_ReturnsEmpty()
    {
        using var app = _appFactory.Create();
        var userContextService = app.Services.GetRequiredService<IUserContextService>();

        var currentUser = await userContextService.GetCurrentContextAsync();

        currentUser.Should().Be(UserContext.Empty);
    }

    [Fact]
    public async Task GetCurrentContextAsync_WhenSignedIn_MapsBasicData()
    {
        using var app = _appFactory.Create();
        var userContextService = app.Services.GetRequiredService<IUserContextService>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
        var userArea = app.SeededEntities.TestUserArea1;

        await userSessionService.SignInAsync(userArea.UserAreaCode, userArea.RoleA.User.UserId, true);
        var currentUser = await userContextService.GetCurrentContextAsync();

        using (new AssertionScope())
        {
            AssertBasicMapping(currentUser, userArea);
        }
    }

    [Fact]
    public async Task GetCurrentContextAsync_WhenSignedInMultiple_ReturnsCorrectUser()
    {
        using var app = _appFactory.Create();
        var userContextService = app.Services.GetRequiredService<IUserContextService>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
        var userArea1 = app.SeededEntities.TestUserArea1;
        var userArea2 = app.SeededEntities.TestUserArea2;

        await userSessionService.SignInAsync(userArea2.UserAreaCode, userArea2.RoleA.User.UserId, true);
        await userSessionService.SignInAsync(userArea1.UserAreaCode, userArea1.RoleA.User.UserId, true);
        var currentUser = await userContextService.GetCurrentContextAsync();

        using (new AssertionScope())
        {
            AssertBasicMapping(currentUser, userArea1);
        }
    }

    [Fact]
    public async Task GetCurrentContextAsync_WhenPasswordChangeRequired_MapsTrue()
    {
        var uniqueData = "GCurCtx_PWCR_MapsTrue";

        using var app = _appFactory.Create();
        var userContextService = app.Services.GetRequiredService<IUserContextService>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
        var userArea = app.SeededEntities.TestUserArea1;
        var userId = await app.TestData.Users().AddAsync(uniqueData, UNIQUE_PREFIX, c => c.RequirePasswordChange = true);

        await userSessionService.SignInAsync(userArea.UserAreaCode, userId, true);
        var currentUser = await userContextService.GetCurrentContextAsync();

        using (new AssertionScope())
        {
            currentUser.IsPasswordChangeRequired.Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetCurrentContextAsync_WhenAccountVerified_MapsTrue()
    {
        var uniqueData = "GCurCtx_AccV_MapsTrue";

        using var app = _appFactory.Create();
        var userContextService = app.Services.GetRequiredService<IUserContextService>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
        var userArea = app.SeededEntities.TestUserArea1;
        var userId = await app.TestData.Users().AddAsync(uniqueData, UNIQUE_PREFIX, c => c.IsAccountVerified = true);

        await userSessionService.SignInAsync(userArea.UserAreaCode, userId, true);
        var currentUser = await userContextService.GetCurrentContextAsync();

        using (new AssertionScope())
        {
            currentUser.IsAccountVerified.Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetSystemUserContextAsync_ReturnsSystemUser()
    {
        using var app = _appFactory.Create();
        var userContextService = app.Services.GetRequiredService<IUserContextService>();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();

        var dbSystemUser = dbContext
            .Users
            .Single(u => u.IsSystemAccount);

        var systemUserContext = await userContextService.GetSystemUserContextAsync();

        using (new AssertionScope())
        {
            systemUserContext.Should().NotBeNull();
            systemUserContext.UserId.Should().Be(dbSystemUser.UserId);
            systemUserContext.RoleCode.Should().Be(SuperAdminRole.Code);
            systemUserContext.RoleId.Should().Be(dbSystemUser.RoleId);
            systemUserContext.UserArea.Should().NotBeNull();
            systemUserContext.UserArea?.UserAreaCode.Should().Be(CofoundryAdminUserArea.Code);
            systemUserContext.IsPasswordChangeRequired.Should().BeFalse();
            systemUserContext.IsAccountVerified.Should().BeFalse();
            systemUserContext.IsCofoundryUser().Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetCurrentContextByUserAreaAsync_NotSignedIn_ReturnsEmpty()
    {
        using var app = _appFactory.Create();
        var userContextService = app.Services.GetRequiredService<IUserContextService>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
        var userArea1 = app.SeededEntities.TestUserArea1;

        var currentUser = await userContextService.GetCurrentContextByUserAreaAsync(userArea1.UserAreaCode);

        currentUser.Should().Be(UserContext.Empty);
    }

    [Fact]
    public async Task GetCurrentContextByUserAreaAsync_WhenSignedInMultiple_ReturnsCorrectUser()
    {
        using var app = _appFactory.Create();
        var userContextService = app.Services.GetRequiredService<IUserContextService>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
        var userArea1 = app.SeededEntities.TestUserArea1;
        var userArea2 = app.SeededEntities.TestUserArea1;

        await userSessionService.SignInAsync(userArea1.UserAreaCode, userArea1.RoleA.User.UserId, true);
        await userSessionService.SignInAsync(userArea2.UserAreaCode, userArea2.RoleA.User.UserId, true);
        var currentUser = await userContextService.GetCurrentContextByUserAreaAsync(userArea1.UserAreaCode);

        using (new AssertionScope())
        {
            AssertBasicMapping(currentUser, userArea2);
        }
    }

    private static void AssertBasicMapping(IUserContext currentUser, SeedData.TestUserAreaInfo userArea)
    {
        currentUser.Should().NotBeNull();
        currentUser.UserId.Should().Be(userArea.RoleA.User.UserId);
        currentUser.RoleCode.Should().Be(userArea.RoleA.RoleCode);
        currentUser.RoleId.Should().Be(userArea.RoleA.RoleId);
        currentUser.UserArea.Should().NotBeNull();
        currentUser.UserArea?.UserAreaCode.Should().Be(userArea.UserAreaCode);
        currentUser.IsPasswordChangeRequired.Should().BeFalse();
        currentUser.IsCofoundryUser().Should().BeFalse();
    }
}
