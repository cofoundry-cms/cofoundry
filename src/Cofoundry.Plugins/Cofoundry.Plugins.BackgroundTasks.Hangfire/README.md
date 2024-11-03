# Cofoundry.Plugins.BackgroundTasks.Hangfire

[![Build status](https://ci.appveyor.com/api/projects/status/7osl8bfowax1yysi?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-backgroundtasks-hangfire)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.BackgroundTasks.Hangfire.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.BackgroundTasks.Hangfire/)


This library is a plugin for [Cofoundry](https://www.cofoundry.org/). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

> HangFire is an easy way to perform background processing in .NET and .NET Core applications. No Windows Service or separate process required.
Backed by persistent storage. Open and free for commercial use.
>
> &mdash; [hangfire.io](http://hangfire.io/)

This [Hangfire](http://hangfire.io/) imlpementation of [Cofoundry.Core.BackgroundTasks](https://github.com/cofoundry-cms/cofoundry/wiki/Background-Tasks) includes:

- A HangFire implementation of `IBackgroundTaskScheduler`
- Automatically initializes HangFire using a [Startup Task](https://github.com/cofoundry-cms/cofoundry/wiki/Startup-Tasks)
- Sets up HangFire to use SqlServer storage using the Cofoundry connection string.
- Optionally sets up to the HangFire dashboard for admin users at **/admin/hangfire**

Cofoundry does not include a background task runner by default so it is recommended that you use this library if you need to run background tasks.

## Settings

- **Cofoundry:Plugins:Hangfire:Disabled** Prevents the HangFire server being configuted and started. Defaults to false.
- **Cofoundry:Plugins:Hangfire:EnableHangfireDashboard** Enables the HangFire dashboard for Cofoundry admin users at */admin/hangfire*. Defaults to false.

## Customizing the HangFire Initialization Process

We use an automatic boostrapper to make HangFire integration simple for most scenarios, but if you want to customize the process you can override the default `IHangfireServerInitializer` implementation using the [Cofoundry DI system](https://github.com/cofoundry-cms/cofoundry/wiki/Dependency-Injection#overriding-registrations). 

This might be required if you want to configure a faster storage engine like [reddis](http://docs.hangfire.io/en/latest/configuration/using-redis.html) for scaling up your deployment.







