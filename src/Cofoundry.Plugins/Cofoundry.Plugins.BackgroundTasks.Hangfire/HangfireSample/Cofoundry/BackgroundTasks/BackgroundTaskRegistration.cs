using Cofoundry.Core.BackgroundTasks;

namespace HangfireSample.Cofoundry.BackgroundTasks;

public class BackgroundTaskRegistration : IBackgroundTaskRegistration
{
    public void Register(IBackgroundTaskScheduler scheduler)
    {
        scheduler.RegisterAsyncRecurringTask<ProductAddingBackgroundTask>(1);
    }
}
