## Official Plugins

### Azure

This library contains services, abstractions and helpers for running in an Azure environment.

- NuGet package: [Cofoundry.Plugins.BackgroundTasks.Hangfire](https://www.nuget.org/packages/Cofoundry.Plugins.Azure)
- [Documentation](Azure)

### BackgroundTasks.Hangfire

Hangfire implementation of Cofoundry.Core.BackgroundTasks. 

- NuGet package: [Cofoundry.Plugins.BackgroundTasks.Hangfire](https://www.nuget.org/packages/Cofoundry.Plugins.BackgroundTasks.Hangfire])
- [Documentation](BackgroundTasks-Hangfire)

### ErrorLogging

This plugin contains a simple implementation of `IErrorLoggingService` that logs errors to a CofoundryPlugin.Error database table and optionally sends an email notification.

- NuGet package: [Cofoundry.Plugins.ErrorLogging](https://www.nuget.org/packages/Cofoundry.Plugins.ErrorLogging)
- [Documentation](ErrorLogging)

### Imaging.ImageSharp

Handle image validation and resizing in Cofoundry using [ImageSharp](https://github.com/SixLabors/ImageSharp), which is licensed under the [Six Labors Split License](https://github.com/SixLabors/ImageSharp/blob/main/LICENSE).

It is fully cross-platform, supports a wide range of formats including animated GIFs and has a comprehensive range of configuration options.

- NuGet package: [Cofoundry.Plugins.Imaging.ImageSharp](https://www.nuget.org/packages/Cofoundry.Plugins.Imaging.ImageSharp)
- [Documentation](Imaging-ImageSharp)

### Imaging.SkiaSharp

Handle image validation and resizing in Cofoundry using [SkiaSharp](https://github.com/mono/SkiaSharp)/[Skia](https://skia.org/), which are MIT/BSD licensed. 

Does not support animated GIF resizing and has limited options for configuration. It is supported on a wide range of platforms but may not be supported in some Linux configurations without a custom build of the native libraries.

- NuGet package: [Cofoundry.Plugins.Imaging.SkiaSharp](https://www.nuget.org/packages/Cofoundry.Plugins.Imaging.SkiaSharp)
- [Documentation](Imaging-SkiaSharp)

### Mail.MailKit

Cofoundry mail services that utilize the cross platform MailKit library. 

- NuGet package: [Cofoundry.Plugins.Mail.MailKit](https://www.nuget.org/packages/Cofoundry.Plugins.Mail.MailKit)
- [Documentation](Mail-MailKit)

### Mail.SendGrid

Cofoundry mail service implemention for SendGrid.

- NuGet package: [Cofoundry.Plugins.Mail.SendGrid](https://www.nuget.org/packages/Cofoundry.Plugins.Mail.SendGrid)
- [Documentation](Mail-SendGrid)

### SiteMap

The SiteMap plugin is a quick and easy way to add a dynamic sitemap to your site. All pages and custom entities routes are added to the sitemap automatically and additional pages can be added easily using an `ISiteMapResourceRegistration` class.

- NuGet package: [Cofoundry.Plugins.SiteMap](https://www.nuget.org/packages/Cofoundry.Plugins.SiteMap)
- [Documentation](SiteMap)

### Vimeo

This plugin adds a single data attribute `[Vimeo]` that can be used to markup a property of type `VimeoVideo`. This will show as a Vimeo Video picker in the admin UI.

- NuGet package: [Cofoundry.Plugins.Vimeo](https://www.nuget.org/packages/Cofoundry.Plugins.Vimeo)
- [Documentation](Vimeo)

### YouTube

This plugin adds a single data attribute `[YouTube]` that can be used to markup a property of type `YouTubeVideo`. This will show as a YouTube Video picker in the admin UI.

- NuGet package: [Cofoundry.Plugins.YouTube](https://www.nuget.org/packages/Cofoundry.Plugins.YouTube)
- [Documentation](YouTube)
