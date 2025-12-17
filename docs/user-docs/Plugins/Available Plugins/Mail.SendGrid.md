This plugin allows you to send mail using the [SendGrid](https://sendgrid.com/) api. 

## Installation

Install the [Cofoundry.Plugins.Mail.SendGrid](https://www.nuget.org/packages/Cofoundry.Plugins.Mail.SendGrid/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.Mail.SendGrid
```

## Configuration

By referencing this package your Cofoundry project will automatically replace the default `IMailDispatchService` implementation with one that uses SendGrid. Use the following settings to configure the service:

- ***Cofoundry:Plugins:SendGrid:ApiKey*** The api key use when authenticating with the SendGrid api.
- ***Cofoundry:Plugins:SendGrid:Disabled*** Indicates whether the plugin should be disabled, which means services will not be bootstrapped. Defaults to false.
