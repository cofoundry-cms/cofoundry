# Cofoundry.Plugins.Mail.MailKit

[![Build status](https://ci.appveyor.com/api/projects/status/2lfyem84399nrwfi?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-mail-mailkit)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.Mail.MailKit.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.Mail.MailKit/)


This library is a plugin for [Cofoundry](https://www.cofoundry.org). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

This library allows you to send mail using the cross platform [MailKit](https://github.com/jstedfast/MailKit) library. By referencing this package your Cofoundry project will automatically replace the default `IMailDispatchService` implementation with one that uses MailKit. Use the following settings to configure the service:

- ***Cofoundry:Plugins:MailKit:Host*** Host address of the smtp server to connect with. Defaults to localhost.
- ***Cofoundry:Plugins:MailKit:Port*** The port to connect to the smtp server on. Defaults to 25.
- ***Cofoundry:Plugins:MailKit:EnableSsl*** Indicates whether ssl should be used to connect to the host.
- ***Cofoundry:Plugins:MailKit:UserName*** The user name to authenticate with the smtp server. If left empty then no auth will be used.
- ***Cofoundry:Plugins:MailKit:Password*** The password use when authenticating with the smtp server.
- ***Cofoundry:Plugins:MailKit:CertificateValidationMode*** Used to configure how the ssl certificate is validated. `Default` uses the default MailKit validator, which allows valid certificates and self-signed certificates with an untrusted root. `All` ignores all certificate errors. `ValidOnly` allows only valid certificates without errors.
- ***Cofoundry:Plugins:MailKit:Disabled*** Indicates whether the plugin should be disabled, which means services will not be bootstrapped. Defaults to false.


## Custom Connection Configuration

For more control over how MailKit initializes and connects to the SMTP host you can implement your own `ISmtpClientConnectionConfiguration` and override the default implementation using the [Cofoundry DI system](https://www.cofoundry.org/docs/framework/dependency-injection).