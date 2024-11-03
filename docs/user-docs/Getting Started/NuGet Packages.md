Cofoundry is split into several packages to make usable in a wide variety of scenarios, not just for websites.

## Main NuGet Packages

For a typical website you'll want to install the [Cofoundry.Admin](https://www.nuget.org/packages/Cofoundry.Web.Admin) NuGet package which will install all of the main packages. Here's a summary of what each package contains.

#### Cofoundry.Core

- Contains all the core framework interfaces and default implementations
- Not usually directly installed unless you're creating a plugin

#### Cofoundry.Domain

- Contains the data and domain models and logic.
- Contains the database installation scripts
- Dependent on Cofoundry.Core
- Not usually installed directly unless you're creating a plugin or non-web project

#### Cofoundry.Web

- Contains website specific functionality, the routes for CMS content and various other helpers and website specific framework hooks
- Dependent on Cofoundry.Domain
- Only installed directly if you don't need the admin panel in your site.

#### Cofoundry.Admin

- Contains the admin panel
- Dependent on Cofoundry.Web
- This is the package you install to get the full Cofoundry web and content editing stack.

## Additional plugin packages

Some features of Cofoundry require additional plugins to be installed in order to use them, this includes [Images](/content-management/images), [Background Tasks](/framework/background-tasks) and [Mail](/framework/mail). Other plugins are available that add additional features to Cofoundry:

- [Azure](https://github.com/cofoundry-cms/Cofoundry.Plugins.Azure)
- [BackgroundTasks.Hangfire](https://github.com/cofoundry-cms/Cofoundry.Plugins.BackgroundTasks.Hangfire)
- [ErrorLogging](https://github.com/cofoundry-cms/Cofoundry.Plugins.ErrorLogging)
- [Imaging.SkiaSharp](https://github.com/cofoundry-cms/Cofoundry.Plugins.Imaging.SkiaSharp)
- [Mail.MailKit](https://github.com/cofoundry-cms/Cofoundry.Plugins.Mail.MailKit)
- [Mail.SendGrid](https://github.com/cofoundry-cms/Cofoundry.Plugins.Mail.SendGrid)
- [SiteMap](https://github.com/cofoundry-cms/Cofoundry.Plugins.SiteMap)
- [Vimeo](https://github.com/cofoundry-cms/Cofoundry.Plugins.Vimeo)
- [YouTube](https://github.com/cofoundry-cms/Cofoundry.Plugins.YouTube)

These plugins are delivered as NuGet packages and are typically self-bootstrapping, making them easy to install and light-up new features.

Check the documentation on each of these areas for more information.