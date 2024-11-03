# Cofoundry.Plugins.ErrorLogging

[![Build status](https://ci.appveyor.com/api/projects/status/r3j6maiudwv42r2d?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-errorlogging)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.ErrorLogging.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.ErrorLogging/)


This library is a plugin for [Cofoundry](https://www.cofoundry.org/). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

This plugin contains a simple implementation of `IErrorLoggingService` that logs errors to a *CofoundryPlugin.Error* database table and optionally sends an email notification.

The [Cofoundry.Plugins.ErrorLogging.Admin](https://www.nuget.org/packages/Cofoundry.Plugins.ErrorLogging/) package can be installed to enable an error log section in the admin panel.

*Note that this current version of the error logging package has not been given much attention. We will be giving this a little more love in upcoming releases.*

## Settings

- **Cofoundry:Plugins:ErrorLogging:LogToEmailAddress** If set, an email notification will be sent to this address every time an error occurs.


