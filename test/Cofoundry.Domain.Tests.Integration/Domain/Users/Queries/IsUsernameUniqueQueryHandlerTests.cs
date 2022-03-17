using Cofoundry.Domain.Tests.Shared.SeedData;

namespace Cofoundry.Domain.Tests.Integration.Users.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class IsUsernameUniqueQueryHandlerTests
{
    const string UNIQUE_PREFIX = "IsPageDirPathUnqQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public IsUsernameUniqueQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    public async Task WhenUsernameUnique_ReturnsTrue()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenUsernameUnique_ReturnsTrue);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var isUnique = await contentRepository
            .Users()
            .IsUsernameUnique(new IsUsernameUniqueQuery()
            {
                UserAreaCode = UserAreaWithoutEmailAsUsername.Code,
                Username = uniqueData
            })
            .ExecuteAsync();

        isUnique.Should().BeTrue();
    }

    public async Task WhenUsernameNotUnique_ReturnsFalse()
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var isUnique = await contentRepository
            .Users()
            .IsUsernameUnique(new IsUsernameUniqueQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Username = app.SeededEntities.AdminUser.Username
            })
            .ExecuteAsync();

        isUnique.Should().BeFalse();
    }

    public async Task WhenExistingUserNotUnique_ReturnsFalse()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenExistingUserNotUnique_ReturnsFalse);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userArea = app.SeededEntities.TestUserArea1;
        var userId = await app.TestData.Users().AddAsync(uniqueData);

        var isUnique = await contentRepository
            .Users()
            .IsUsernameUnique(new IsUsernameUniqueQuery()
            {
                UserAreaCode = userArea.UserAreaCode,
                Username = userArea.RoleA.User.Username,
                UserId = userId
            })
            .ExecuteAsync();

        isUnique.Should().BeFalse();
    }

    public async Task WhenExistingUserNotChanged_ReturnsTrue()
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var isUnique = await contentRepository
            .Users()
            .IsUsernameUnique(new IsUsernameUniqueQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Username = app.SeededEntities.AdminUser.Username,
                UserId = app.SeededEntities.AdminUser.UserId
            })
            .ExecuteAsync();

        isUnique.Should().BeTrue();
    }

    public async Task WhenExistingUserChangedToUnique_ReturnsTrue()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenUsernameUnique_ReturnsTrue);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var isUnique = await contentRepository
            .Users()
            .IsUsernameUnique(new IsUsernameUniqueQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Username = uniqueData + "@example.com",
                UserId = app.SeededEntities.AdminUser.UserId
            })
            .ExecuteAsync();

        isUnique.Should().BeTrue();
    }
}
