## Official Plugins

### [Cofoundry.Plugins.Azure](https://github.com/cofoundry-cms/Cofoundry.Plugins.Azure)

This library contains services, abstractions and helpers for running in an Azure environment.

### [Cofoundry.Plugins.BackgroundTasks.Hangfire](https://github.com/cofoundry-cms/Cofoundry.Plugins.BackgroundTasks.Hangfire)

Hangfire implementation of Cofoundry.Core.BackgroundTasks. 

### [Cofoundry.Plugins.ErrorLogging](https://github.com/cofoundry-cms/Cofoundry.Plugins.ErrorLogging)

This plugin contains a simple implementation of `IErrorLoggingService` that logs errors to a CofoundryPlugin.Error database table and optionally sends an email notification.

###  [Cofoundry.Plugins.Imaging.ImageSharp](https://github.com/cofoundry-cms/Cofoundry.Plugins.Imaging.ImageSharp)

Handle image validation and resizing in Cofoundry using [ImageSharp](https://github.com/SixLabors/ImageSharp), which is licensed under the [Six Labors Split License](https://github.com/SixLabors/ImageSharp/blob/main/LICENSE).

It is fully cross-platform, supports a wide range of formats including animated GIFs and has a comprehensive range of configuration options.

###  [Cofoundry.Plugins.Imaging.SkiaSharp](https://github.com/cofoundry-cms/Cofoundry.Plugins.Imaging.SkiaSharp)

Handle image validation and resizing in Cofoundry using [SkiaSharp](https://github.com/mono/SkiaSharp)/[Skia](https://skia.org/), which are MIT/BSD licensed. 

Does not support animated GIF resizing and has limited options for configuration. It is supported on a wide range of platforms but may not be supported in some Linux configurations without a custom build of the native libraries.

### [Cofoundry.Plugins.Mail.MailKit](https://github.com/cofoundry-cms/Cofoundry.Plugins.Mail.MailKit)

Cofoundry mail services that utilize the cross platform MailKit library. 

### [Cofoundry.Plugins.Mail.SendGrid](https://github.com/cofoundry-cms/Cofoundry.Plugins.Mail.SendGrid)

Cofoundry mail service implemention for SendGrid.

### [Cofoundry.Plugins.SiteMap](https://github.com/cofoundry-cms/Cofoundry.Plugins.SiteMap)

The SiteMap plugin is a quick and easy way to add a dynamic sitemap to your site. All pages and custom entities routes are added to the sitemap automatically and additional pages can be added easily using an `ISiteMapResourceRegistration` class.

### [Cofoundry.Plugins.Vimeo](https://github.com/cofoundry-cms/Cofoundry.Plugins.Vimeo)

This plugin adds a single data attribute `[Vimeo]` that can be used to markup a property of type `VimeoVideo`. This will show as a Vimeo Video picker in the admin UI.

### [Cofoundry.Plugins.YouTube](https://github.com/cofoundry-cms/Cofoundry.Plugins.YouTube)

This plugin adds a single data attribute `[YouTube]` that can be used to markup a property of type `YouTubeVideo`. This will show as a YouTube Video picker in the admin UI.
