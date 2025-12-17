This plugin uses the [SkiaSharp](https://github.com/mono/SkiaSharp) wrapper library to resize images dynamically. [Skia](https://skia.org/) is an open source cross platform a 2D graphics library sponsored and managed by Google. It's licenced under the BSD Free Software License and is used in products such as Chrome, Android and FireFox. 

## Installation

Install the [Cofoundry.Plugins.Imaging.SkiaSharp](https://www.nuget.org/packages/Cofoundry.Plugins.Imaging.SkiaSharp/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.Imaging.SkiaSharp
```

## Configuration

The services register themselves automatically so typically no other configuration is required. The following configuration settings can be used to tweak the image output:

- **Cofoundry:Plugins:SkiaSharp:JpegQuality** Jpeg quality setting out of 100. Defaults to 85.
- **Cofoundry:Plugins:SkiaSharp:GifResizeBehaviour** Gifs can be saved but not resized. Use this to customize the  fallback behaviour. Options are: 
  - Auto: Save single frame gifs as PNGs, but leave animated gifs untouched and do not resize them.
  - NoResize: Dont resize, output the original image instead.
  - ResizeAsAlternative: Re-encoded all gifs to pngs when they are uploaded.
- **Cofoundry:Plugins:SkiaSharp:DisableFileCache** Disables reading and writing of caching of resized image files. Useful for debugging, but note that cache headers are still set and files may be cached upstream.

SkiaSharp doesn't offer much control over how images are saved, so if you need more control over configuration or better gif support you might want to consider the [Cofoundry.Plugins.Imaging.ImageSharp](https://www.nuget.org/packages/Cofoundry.Plugins.Imaging.ImageSharp) library.
