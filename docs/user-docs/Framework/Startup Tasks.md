In order for Cofoundry to correctly configure itself with the hosting environment it needs to control the order in which certain configuration tasks are run. If you too want to tap into this startup process you can implement your own startup task classes.

This is particularly useful if you're creating a plugin or just want to keep your application code modular.

## Types of Startup Tasks

There are two types of startup tasks, one for each part of the .net core startup process:

- **IStartupServiceConfigurationTask:** These tasks run during the `ConfigureServices` block of your application startup. They run after `AddMvc()` has been called and provide you `IMvcBuilder` as a parameter to act on.
- **IStartupConfigurationTask:** These tasks run during the `Configure` block of your application startup, which is typically where middleware gets registered. The tasks get passed an instance of `IApplicationBuilder`.


## Creating a Startup Task

Creating a startup task is as simple as creating a class that implements either `IStartupServiceConfigurationTask` or `IStartupConfigurationTask`. Startup tasks are registered automatically with the DI container so you can take advantage of constructor injection. Here's an example of our startup task that bootstraps the `IMessageAggregator` framework:

```csharp
using Cofoundry.Web;

public class MessageAggregatorStartupConfigurationTask : IStartupConfigurationTask
{
    private readonly IMessageSubscriptionInitializer _messageSubscriptionInitializer;

    public MessageAggregatorStartupConfigurationTask(
        IMessageSubscriptionInitializer messageSubscriptionInitializer
        )
    {
        _messageSubscriptionInitializer = messageSubscriptionInitializer;
    }

    public int Ordering => (int)StartupTaskOrdering.Normal;

    public void Configure(IApplicationBuilder app)
    {
        _messageSubscriptionInitializer.Initialize();
    }
}
```

## Startup Task Ordering

The `Ordering` property allows us to customize which order the tasks run in. Although it is possible to use an integer value, this should be used as a last resort and more commonly you should use the the values of the `StartupTaskOrdering` enum.

If you want to be more specific and have your task be dependent on a different task you can implement either:

- **IRunBeforeStartupConfigurationTask:** Run your task before one or more dependent tasks
- **IRunAfterStartupConfigurationTask:** Ensure you task runs before one or more tasks.


