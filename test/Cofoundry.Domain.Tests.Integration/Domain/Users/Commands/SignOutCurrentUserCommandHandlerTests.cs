using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class SignOutCurrentUserCommandHandlerTests
{
    const string UNIQUE_PREFIX = "SignOCurUsrCHT-";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public SignOutCurrentUserCommandHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task WhenSignedInDefaultUserArea_CanSignOut()
    {
        var uniqueData = UNIQUE_PREFIX + "SIDef_SO";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();

        var userId = await app.TestData.Users().AddAsync(uniqueData);
        await userSessionService.SignInAsync(TestUserArea1.Code, userId, true);
        var userIdBeforeSignOut = userSessionService.GetCurrentUserId();

        await contentRepository
            .Users()
            .Authentication()
            .SignOutAsync();

        var userIdAfterSignOut = userSessionService.GetCurrentUserId();

        using (new AssertionScope())
        {
            userIdBeforeSignOut.Should().Be(userId);
            userIdAfterSignOut.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenSignedInNonDefaultUserArea_SignOutDefaultDoesNotSignOut()
    {
        var uniqueData = UNIQUE_PREFIX + "SINDef_DefNotSO";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
        var userArea = app.SeededEntities.TestUserArea2;
        var userId = await app.TestData.Users().AddAsync(uniqueData, c =>
        {
            c.UserAreaCode = userArea.UserAreaCode;
            c.RoleId = userArea.RoleA.RoleId;
        });
        await userSessionService.SignInAsync(userArea.UserAreaCode, userId, true);
        var userIdBeforeSignOut = await userSessionService.GetUserIdByUserAreaCodeAsync(userArea.UserAreaCode);

        await contentRepository
            .Users()
            .Authentication()
            .SignOutAsync();

        var userIdAfterSignOut = await userSessionService.GetUserIdByUserAreaCodeAsync(userArea.UserAreaCode);

        using (new AssertionScope())
        {
            userIdBeforeSignOut.Should().Be(userId);
            userIdAfterSignOut.Should().Be(userIdBeforeSignOut);
        }
    }

    [Fact]
    public async Task WhenSignedInNonDefaultUserArea_CanSignOut()
    {
        var uniqueData = UNIQUE_PREFIX + "SINDef_SO";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();
        var userArea = app.SeededEntities.TestUserArea2;

        var userId = await app.TestData.Users().AddAsync(uniqueData, c =>
        {
            c.UserAreaCode = userArea.UserAreaCode;
            c.RoleId = userArea.RoleA.RoleId;
        });
        await userSessionService.SignInAsync(userArea.UserAreaCode, userId, true);
        var userIdBeforeSignOut = await userSessionService.GetUserIdByUserAreaCodeAsync(userArea.UserAreaCode);
        await contentRepository
            .WithContext<TestUserArea2>()
            .Users()
            .Authentication()
            .SignOutAsync();

        var userIdAfterSignOut = await userSessionService.GetUserIdByUserAreaCodeAsync(userArea.UserAreaCode);

        using (new AssertionScope())
        {
            userIdBeforeSignOut.Should().Be(userId);
            userIdAfterSignOut.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenSignedOut_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + "SO_Message";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var userSessionService = app.Services.GetRequiredService<IUserSessionService>();

        var userId = await app.TestData.Users().AddAsync(uniqueData);
        await userSessionService.SignInAsync(TestUserArea1.Code, userId, true);

        await contentRepository
            .Users()
            .Authentication()
            .SignOutAsync();

        app.Mocks
            .CountMessagesPublished<UserSignedOutMessage>(m =>
            {
                return m.UserId == userId && m.UserAreaCode == TestUserArea1.Code;
            })
            .Should().Be(1);
    }

    [Fact]
    public async Task WhenNotSignedIn_DoesNotMessage()
    {
        var uniqueData = UNIQUE_PREFIX + "NSIN_NoMessage";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();

        var userId = await app.TestData.Users().AddAsync(uniqueData);

        await contentRepository
            .Users()
            .Authentication()
            .SignOutAsync();

        app.Mocks
            .CountMessagesPublished<UserSignedOutMessage>()
            .Should().Be(0);
    }
}
