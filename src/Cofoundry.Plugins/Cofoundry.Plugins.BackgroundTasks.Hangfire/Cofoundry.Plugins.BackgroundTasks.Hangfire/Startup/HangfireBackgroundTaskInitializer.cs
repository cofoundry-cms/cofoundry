using Cofoundry.Core.BackgroundTasks;

namespace Cofoundry.Plugins.BackgroundTasks.Hangfire;

/// <summary>
/// Used to bootstrap self-registering background tasks.
/// </summary>
public class HangfireBackgroundTaskInitializer : IHangfireBackgroundTaskInitializer
{
    private readonly HangfireSettings _hangfireSettings;
    private readonly IEnumerable<IBackgroundTaskRegistration> _backgroundTaskRegistrations;
    private readonly IBackgroundTaskScheduler _backgroundTaskScheduler;

    public HangfireBackgroundTaskInitializer(
        HangfireSettings hangfireSettings,
        IEnumerable<IBackgroundTaskRegistration> backgroundTaskRegistrations,
        IBackgroundTaskScheduler backgroundTaskScheduler
        )
    {
        _hangfireSettings = hangfireSettings;
        _backgroundTaskRegistrations = backgroundTaskRegistrations;
        _backgroundTaskScheduler = backgroundTaskScheduler;
    }

    public void Initialize()
    {
        if (_hangfireSettings.Disabled)
        {
            return;
        }

        foreach (var registration in _backgroundTaskRegistrations)
        {
            registration.Register(_backgroundTaskScheduler);
        }
    }
}
