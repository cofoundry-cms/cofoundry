using Cofoundry.Core.BackgroundTasks;
using Cofoundry.Domain.BackgroundTasks;

namespace Cofoundry.Domain.Registration
{
    public class UsersBackgroundTaskRegistration : IBackgroundTaskRegistration
    {
        private readonly UsersSettings _usersSettings;

        public UsersBackgroundTaskRegistration(
            UsersSettings usersSettings
            )
        {
            _usersSettings = usersSettings;
        }

        public void Register(IBackgroundTaskScheduler scheduler)
        {
            scheduler.RegisterAsyncRecurringTask<UserCleanupBackgroundTask>(_usersSettings.Cleanup.BackgroundTaskFrequencyInMinutes, 0);
        }
    }
}