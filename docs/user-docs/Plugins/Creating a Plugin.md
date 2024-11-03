Cofoundry is built upon a modular foundation that can be used by plugin authors to swap out or add additional features to a Cofoundry application by simply installing a NuGet package.

A core part of this is our [Dependency Injection](/framework/dependency-injection) system that automatically scans for classes implementing Cofoundry self-registering features. The framework can also be used to override base implementations of services to provide expanded or customized capabilities.

Additionally Cofoundry has a number of features that help make the ASP.NET Core framework more modular, allowing you to serve up embedded views and content from a NuGet package or project reference.

*We plan to document this area more fully in the future, but to get a flavor of how this all works, take a look at the source code for existing plugins such as [Mail.SendGrid](https://github.com/cofoundry-cms/Cofoundry.Plugins.Mail.SendGrid), [Azure](https://github.com/cofoundry-cms/Cofoundry.Plugins.Azure) and [ErrorLogging](https://github.com/cofoundry-cms/Cofoundry.Plugins.ErrorLogging)*