Cofoundry has a built-in abstraction of background task registrations, this enables the framework and any plugins to use the same interface for registering background tasks while giving you the freedom to choose how those tasks are run. 

Currently there isn't default implementation included in the main package, instead you'll need to install a plugin like [Cofoundry.Plugins.BackgroundTasks.Hangfire](https://github.com/cofoundry-cms/Cofoundry.Plugins.BackgroundTasks.Hangfire) to get them to run.

## Creating a Recurring Background Task

Simply create a class that inherits from either `IRecurringBackgroundTask` or `IAsyncRecurringBackgroundTask` and implement the `Execute[Async]` method. [BackgroundTasks.Hangfire](https://github.com/cofoundry-cms/Cofoundry.Plugins.BackgroundTasks.Hangfire) supports constructor injection so typically the background task will be lightweight and we can just execute a command using a CommandExecutor. E.g.

```csharp
using Cofoundry.Core.BackgroundTasks;

public class ImportDataBackgroundTask : IAsyncRecurringBackgroundTask
{
    private readonly IDomainRepository _domainRepository;

    public ImportDataBackgroundTask(
        IDomainRepository domainRepository
        )
    {
        _domainRepository = domainRepository;
    }

    public Task ExecuteAsync()
    {
        var command = new ImportDataCommand();
        return _domainRepository.ExecuteCommandAsync(command);
    }
}
```

Once this has been created you'll need to register your background task by creating a registration class that implements `IBackgroundTaskRegistration`. You can then use the `IBackgroundTaskScheduler` to schedule your tasks to run at a specific interval. Note that the smallest interval is 1 minute, and that execution times are UTC time. Example:

```csharp
using Cofoundry.Core.BackgroundTasks;

public class BackgroundTaskRegistration : IBackgroundTaskRegistration
{
    public void Register(IBackgroundTaskScheduler scheduler)
    {
        scheduler
            .RegisterRecurringTask<ImportDataBackgroundTask>(1, 6, 5);
    }
}
```

## Other Task Types

Only recurring tasks are currently implemented currently but it would be fairly easy to extend the framework to allow ad-hoc background tasks to be queued and executed on a background thread if you need it.