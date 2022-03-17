using Cofoundry.Core.BackgroundTasks;

namespace Cofoundry.Domain.BackgroundTasks;

public class AuthorizedTaskCleanupBackgroundTask : IAsyncRecurringBackgroundTask
{
    private readonly IDomainRepository _domainRepository;
    private readonly AuthorizedTaskCleanupSettings _authorizedTaskCleanupSettings;

    public AuthorizedTaskCleanupBackgroundTask(
        IDomainRepository domainRepository,
        AuthorizedTaskCleanupSettings authorizedTaskCleanupSettings
        )
    {
        _domainRepository = domainRepository;
        _authorizedTaskCleanupSettings = authorizedTaskCleanupSettings;
    }

    public async Task ExecuteAsync()
    {
        if (!_authorizedTaskCleanupSettings.Enabled
            || !_authorizedTaskCleanupSettings.RetentionPeriodInDays.HasValue
            || _authorizedTaskCleanupSettings.RetentionPeriodInDays < 0)
        {
            return;
        }

        await _domainRepository
            .WithElevatedPermissions()
            .ExecuteCommandAsync(new CleanupAuthorizedTasksCommand()
            {
                RetentionPeriod = TimeSpan.FromDays(_authorizedTaskCleanupSettings.RetentionPeriodInDays.Value)
            });
    }
}
