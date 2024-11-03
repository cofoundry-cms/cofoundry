# Cofoundry.Plugins.Imaging.SkiaSharp

[![Build status](https://ci.appveyor.com/api/projects/status/usl4w7v90xorrd98?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-imaging-skiasharp)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.Imaging.SkiaSharp.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.Imaging.SkiaSharp/)

This library is a plugin for [Cofoundry](https://www.cofoundry.org). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

Cofoundry does not have a default image resizing implementation and relies on plugins to add this functionality. For more info on image resizing in Cofoundry check out the [imaging documentation](https://github.com/cofoundry-cms/cofoundry/wiki/Images). 

This plugin uses the [SkiaSharp](https://github.com/mono/SkiaSharp) wrapper library to resize images dynamically. [Skia](https://skia.org/) is an open source cross platform a 2D graphics library sponsored and managed by Google. It's licenced under the BSD Free Software License and is used in products such as Chrome, Android and FireFox. 

## Configuration

The services register themselves automatically so typically no other configuration is required. The following configuration settings can be used to tweak the image output:

- **Cofoundry:Plugins:SkiaSharp:JpegQuality** Jpeg quality setting out of 100. Defaults to 85.
- **Cofoundry:Plugins:SkiaSharp:GifResizeBehaviour** Gifs can be saved but not resized. Use this to customize the  fallback behaviour. Options are: 
  - Auto: Save single frame gifs as PNGs, but leave animated gifs untouched and do not resize them.
  - NoResize: Dont resize, output the original image instead.
  - ResizeAsAlternative: Re-encoded all gifs to pngs when they are uploaded.
- **Cofoundry:Plugins:SkiaSharp:DisableFileCache** Disables reading and writing of caching of resized image files. Useful for debugging, but note that cache headers are still set and files may be cached upstream.

SkiaSharp doesn't offer much control over how images are saved, so if you need more control over configuration or better gif support you might want to consider the [Cofoundry.Plugins.Imaging.ImageSharp](https://github.com/cofoundry-cms/Cofoundry.Plugins.Imaging.ImageSharp) library.