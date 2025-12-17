> HangFire is an easy way to perform background processing in .NET and .NET Core applications. No Windows Service or separate process required.
Backed by persistent storage. Open and free for commercial use.
>
> &mdash; [hangfire.io](http://hangfire.io/)

This [Hangfire](http://hangfire.io/) imlpementation of [Cofoundry.Core.BackgroundTasks](https://www.cofoundry.org/docs/framework/background-tasks) includes:

- A HangFire implementation of `IBackgroundTaskScheduler`
- Automatically initializes HangFire using a [Startup Task](https://www.cofoundry.org/docs/framework/startup-tasks)
- Sets up HangFire to use SqlServer storage using the Cofoundry connection string.
- Optionally sets up to the HangFire dashboard for admin users at **/admin/hangfire**

Cofoundry does not include a background task runner by default so it is recommended that you use this library if you need to run background tasks.

## Installation

Install the [Cofoundry.Plugins.BackgroundTasks.Hangfire](https://www.nuget.org/packages/Cofoundry.Plugins.BackgroundTasks.Hangfire/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.BackgroundTasks.Hangfire
```

## Configuration

- **Cofoundry:Plugins:Hangfire:Disabled** Prevents the HangFire server being configuted and started. Defaults to false.
- **Cofoundry:Plugins:Hangfire:EnableHangfireDashboard** Enables the HangFire dashboard for Cofoundry admin users at */admin/hangfire*. Defaults to false.

### Customizing the HangFire Initialization Process

We use an automatic boostrapper to make HangFire integration simple for most scenarios, but if you want to customize the process you can override the default `IHangfireServerInitializer` implementation using the [Cofoundry DI system](https://www.cofoundry.org/docs/framework/dependency-injection#overriding-registrations). 

This might be required if you want to configure a faster storage engine like [reddis](http://docs.hangfire.io/en/latest/configuration/using-redis.html) for scaling up your deployment.

