using Cofoundry.Core.BackgroundTasks;
using Cofoundry.Domain.BackgroundTasks;

namespace Cofoundry.Domain.Registration
{
    public class AuthorizedTasksBackgroundTaskRegistration : IBackgroundTaskRegistration
    {
        private readonly AuthorizedTaskCleanupSettings _authorizedTaskCleanupSettings;

        public AuthorizedTasksBackgroundTaskRegistration(
            AuthorizedTaskCleanupSettings authorizedTaskCleanupSettings
            )
        {
            _authorizedTaskCleanupSettings = authorizedTaskCleanupSettings;
        }

        public void Register(IBackgroundTaskScheduler scheduler)
        {
            scheduler.RegisterAsyncRecurringTask<AssetFileCleanupBackgroundTask>(_authorizedTaskCleanupSettings.BackgroundTaskFrequencyInHours, 0);
        }
    }
}