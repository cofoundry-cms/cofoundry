using Cofoundry.Domain.Tests.Shared;

namespace Cofoundry.Domain.Tests.Integration.Users.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class HasExceededMaxAuthenticationAttemptsQueryHandlerTests
{
    const string UNIQUE_PREFIX = "HEMaxAuthAttQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public HasExceededMaxAuthenticationAttemptsQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    public async Task WhenNoFailures_ReturnsTrue()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenNoFailures_ReturnsTrue);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        app.Mocks.MockIPAddress(uniqueData);

        var hasExceeded = await contentRepository
            .ExecuteQueryAsync(new HasExceededMaxAuthenticationAttemptsQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = uniqueData
            });

        hasExceeded.Should().BeFalse();
    }

    public async Task WhenIPFailure_ReturnsFalse()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenIPFailure_ReturnsFalse);
        var now = new DateTime(1962, 5, 4);

        using var app = _appFactory.Create(s => s.Configure<UsersSettings>(o =>
        {
            o.Authentication.IPAddressRateLimit = new RateLimitConfiguration(2, TimeSpan.FromMinutes(3));
        }));

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        app.Mocks.MockIPAddress(uniqueData);

        app.Mocks.MockDateTime(now);
        var hasExceededAfter1 = await RunAttemptAsync(1);

        app.Mocks.MockDateTime(now.AddMinutes(1));
        var hasExceededAfter2 = await RunAttemptAsync(2);

        app.Mocks.MockDateTime(now.AddMinutes(2));
        var hasExceededAfter3 = await RunAttemptAsync(3);

        app.Mocks.MockDateTime(now.AddMinutes(4));
        var hasExceededAfter4 = await RunAttemptAsync(4);

        using (new AssertionScope())
        {
            hasExceededAfter1.Should().BeFalse();
            hasExceededAfter2.Should().BeFalse();
            hasExceededAfter3.Should().BeTrue();
            hasExceededAfter3.Should().BeFalse();
        }

        async Task<bool> RunAttemptAsync(int attemptNum)
        {
            await contentRepository
                .ExecuteCommandAsync(new LogFailedAuthenticationAttemptCommand()
                {
                    UserAreaCode = TestUserArea1.Code,
                    Username = uniqueData + attemptNum
                });

            return await contentRepository
            .ExecuteQueryAsync(new HasExceededMaxAuthenticationAttemptsQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = uniqueData + attemptNum
            });
        }
    }

    public async Task WhenUsernameFailure_ReturnsFalse()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenIPFailure_ReturnsFalse);
        var now = new DateTime(1962, 5, 5);

        using var app = _appFactory.Create(s => s.Configure<UsersSettings>(o =>
        {
            o.Authentication.UsernameRateLimit = new RateLimitConfiguration(2, TimeSpan.FromMinutes(3));
        }));

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        app.Mocks.MockDateTime(now);
        var hasExceededAfter1 = await RunAttemptAsync(1);

        app.Mocks.MockDateTime(now.AddMinutes(1));
        var hasExceededAfter2 = await RunAttemptAsync(2);

        app.Mocks.MockDateTime(now.AddMinutes(2));
        var hasExceededAfter3 = await RunAttemptAsync(3);

        app.Mocks.MockDateTime(now.AddMinutes(4));
        var hasExceededAfter4 = await RunAttemptAsync(4);

        using (new AssertionScope())
        {
            hasExceededAfter1.Should().BeFalse();
            hasExceededAfter2.Should().BeFalse();
            hasExceededAfter3.Should().BeTrue();
            hasExceededAfter3.Should().BeFalse();
        }

        async Task<bool> RunAttemptAsync(int attemptNum)
        {
            app.Mocks.MockIPAddress(uniqueData + attemptNum);

            await contentRepository
                .ExecuteCommandAsync(new LogFailedAuthenticationAttemptCommand()
                {
                    UserAreaCode = TestUserArea1.Code,
                    Username = uniqueData
                });

            return await contentRepository
            .ExecuteQueryAsync(new HasExceededMaxAuthenticationAttemptsQuery()
            {
                UserAreaCode = TestUserArea1.Code,
                Username = uniqueData
            });
        }
    }
}
