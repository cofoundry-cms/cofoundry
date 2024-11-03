using Cofoundry.Core.BackgroundTasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Plugins.BackgroundTasks.Hangfire;

public class HangfireBackgroundTasksDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IHangfireBackgroundTaskInitializer, HangfireBackgroundTaskInitializer>()
            .Register<IHangfireServerInitializer, HangfireServerInitializer>()
            .Register<IBackgroundTaskScheduler, HangfireBackgroundTaskScheduler>()
            ;
    }
}
