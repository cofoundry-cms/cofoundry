using Cofoundry.Domain.BackgroundTasks;

namespace Cofoundry.Domain.Tests.Integration.AuthorizedTasks.BackgroundTasks;

[Collection(nameof(DbDependentFixtureCollection))]
public class AuthorizedTaskCleanupBackgroundTaskTests
{
    private readonly DbDependentTestApplicationFactory _appFactory;

    public AuthorizedTaskCleanupBackgroundTaskTests(
         DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task WhenEnabled_Runs()
    {
        CleanupAuthorizedTasksCommand executedCommand = null;
        using var app = _appFactory.Create(s =>
        {
            s.Configure<AuthorizedTaskCleanupSettings>(c => c.RetentionPeriodInDays = 11);
            s.MockHandler<CleanupAuthorizedTasksCommand>(c => executedCommand = c);
        });

        var backgroundTask = app.Services.GetRequiredService<AuthorizedTaskCleanupBackgroundTask>();
        await backgroundTask.ExecuteAsync();

        using (new AssertionScope())
        {
            executedCommand.Should().NotBeNull();
            executedCommand.RetentionPeriod.Should().Be(TimeSpan.FromDays(11));
        }
    }

    [Fact]
    public async Task WhenDisabled_DoesNotRun()
    {
        CleanupAuthorizedTasksCommand executedCommand = null;
        using var app = _appFactory.Create(s =>
        {
            s.Configure<AuthorizedTaskCleanupSettings>(c => c.Enabled = false);
            s.MockHandler<CleanupAuthorizedTasksCommand>(c => executedCommand = c);
        });

        var backgroundTask = app.Services.GetRequiredService<AuthorizedTaskCleanupBackgroundTask>();
        await backgroundTask.ExecuteAsync();

        using (new AssertionScope())
        {
            executedCommand.Should().BeNull();
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData(-1)]
    public async Task WhenInvalidRetentionPeriod_DoesNotRun(int? retentionPeriodInDays)
    {
        CleanupAuthorizedTasksCommand executedCommand = null;
        using var app = _appFactory.Create(s =>
        {
            s.Configure<AuthorizedTaskCleanupSettings>(c => c.RetentionPeriodInDays = retentionPeriodInDays);
            s.MockHandler<CleanupAuthorizedTasksCommand>(c => executedCommand = c);
        });

        var backgroundTask = app.Services.GetRequiredService<AuthorizedTaskCleanupBackgroundTask>();
        await backgroundTask.ExecuteAsync();

        using (new AssertionScope())
        {
            executedCommand.Should().BeNull();
        }
    }
}
