This plugin uses the cross platform [ImageSharp](https://github.com/SixLabors/ImageSharp) package to resize images dynamically. ImageSharp has a number of benefits over the [SkiaSharp imaging plugin](Imaging-SkiaSharp):

- Support for (animated) GIFs
- An extensive range of configuration options
- The ability to preserve EXIF data
- Fully cross-platform
- A commercial support model

ImageSharp is licensed under the [Six Labors Split License](https://github.com/SixLabors/ImageSharp/blob/main/LICENSE), see [the Six Labors website for more information on licensing options](https://sixlabors.com/).

## Installation

Install the [Cofoundry.Plugins.Imaging.ImageSharp](https://www.nuget.org/packages/Cofoundry.Plugins.Imaging.ImageSharp/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.Imaging.ImageSharp
```

## Configuration

The services register themselves automatically so typically no other configuration is required. The following configuration settings can be used to tweak the image output:

- **Cofoundry:Plugins:ImageSharp:JpegQuality** Jpeg quality setting out of 100. Defaults to *85*.
- **Cofoundry:Plugins:ImageSharp:IgnoreMetadata** Indicates whether the metadata should be ignored when the image is being encoded. Defaults to *true*.

If you need more control over the image configuration this can be achieved by changing the ImageSharp configuration manually. Check out the  [ChangeDefaultEncoderOptions](https://github.com/SixLabors/ImageSharp/tree/master/samples/ChangeDefaultEncoderOptions) sample for more information. Our default configuration is set during the Cofoundry startup process, so you'll need to apply your settings after you've called `app.UseCofoundry();`.
