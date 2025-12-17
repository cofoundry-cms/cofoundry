This plugin contains a simple implementation of `IErrorLoggingService` that logs errors to a *CofoundryPlugin.Error* database table and optionally sends an email notification. 

In a web project you can use the [Cofoundry.Plugins.ErrorLogging.Admin](https://www.nuget.org/packages/Cofoundry.Plugins.ErrorLogging.Admin/) package to add an error log section in the admin panel.

*Note that this current version of the error logging package has not been given much attention. We will be giving this a little more love in upcoming releases.*

## Installation

Install the [Cofoundry.Plugins.ErrorLogging.Admin](https://www.nuget.org/packages/Cofoundry.Plugins.ErrorLogging.Admin/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.ErrorLogging.Admin
```

## Configuration

- **Cofoundry:Plugins:ErrorLogging:LogToEmailAddress** If set, an email notification will be sent to this address every time an error occurs.


