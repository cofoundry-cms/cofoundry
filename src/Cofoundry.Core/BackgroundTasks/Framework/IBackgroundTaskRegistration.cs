namespace Cofoundry.Core.BackgroundTasks;

/// <summary>
/// Implement this to allow automatic registration of background tasks
/// during a boostrap process.
/// </summary>
public interface IBackgroundTaskRegistration
{
    void Register(IBackgroundTaskScheduler scheduler);
}
