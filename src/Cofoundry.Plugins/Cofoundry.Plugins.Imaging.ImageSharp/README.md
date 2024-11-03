# Cofoundry.Plugins.Imaging.ImageSharp

[![Build status](https://ci.appveyor.com/api/projects/status/neoc6yy7ed64td14?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-imaging-imagesharp)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.Imaging.ImageSharp.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.Imaging.ImageSharp/)

This library is a plugin for [Cofoundry](https://www.cofoundry.org). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

Cofoundry does not have a default image resizing implementation and relies on plugins to add this functionality. For more info on image resizing in Cofoundry check out the [imaging documentation](https://github.com/cofoundry-cms/cofoundry/wiki/Images). 

This plugin uses the cross platform [ImageSharp](https://github.com/SixLabors/ImageSharp) package to resize images dynamically. ImageSharp has a number of benefits over our  [SkiaSharp imaging package](https://github.com/cofoundry-cms/Cofoundry.Plugins.Imaging.SkiaSharp):

- Support for (animated) GIFs
- An extensive range of configuration options
- The ability to preserve EXIF data
- Fully cross-platform
- A commercial support model

ImageSharp is licensed under the [Six Labors Split License](https://github.com/SixLabors/ImageSharp/blob/main/LICENSE), see [the Six Labors website for more information on licensing options](https://sixlabors.com/).

## Configuration

The services register themselves automatically so typically no other configuration is required. The following configuration settings can be used to tweak the image output:

- **Cofoundry:Plugins:ImageSharp:JpegQuality** Jpeg quality setting out of 100. Defaults to *85*.
- **Cofoundry:Plugins:ImageSharp:IgnoreMetadata** Indicates whether the metadata should be ignored when the image is being encoded. Defaults to *true*.

If you need more control over the image configuration this can be achieved by changing the ImageSharp configuration manually. Check out the  [ChangeDefaultEncoderOptions](https://github.com/SixLabors/ImageSharp/tree/master/samples/ChangeDefaultEncoderOptions) sample for more information. Our default configuration is set during the Cofoundry startup process, so you'll need to apply your settings after you've called `app.UseCofoundry();`.