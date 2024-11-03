# Cofoundry.Plugins.Mail.SendGrid

[![Build status](https://ci.appveyor.com/api/projects/status/wquf931go22ibjg6?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-mail-sendgrid)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.Mail.SendGrid.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.Mail.SendGrid/)


This library is a plugin for [Cofoundry](https://www.cofoundry.org/). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

This plugin allows you to send mail using the [SendGrid](https://sendgrid.com/) api. By referencing this package your Cofoundry project will automatically replace the default IMailDispatchService implementation with one that uses SendGrid. Use the following settings to configure the service:

- ***Cofoundry:Plugins:SendGrid:ApiKey*** The api key use when authenticating with the SendGrid api.
- ***Cofoundry:Plugins:SendGrid:Disabled*** Indicates whether the plugin should be disabled, which means services will not be bootstrapped. Defaults to false.





