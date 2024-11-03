## Managing Image Assets

The admin panel has a whole section devoted to managing images where users can add, update and delete images assets from a central place.

## Supporting Image Assets

Images assets are only supported by installing a plugin, this is because there isn't a good option for working with image files in .NET without using a 3rd party library.

Currently there are two plugin options:

- [Cofoundry.Plugins.Imaging.SkiaSharp](https://github.com/cofoundry-cms/Cofoundry.Plugins.Imaging.SkiaSharp): Uses the [SkiaSharp](https://github.com/mono/SkiaSharp)/[Skia](https://skia.org/) libraries which are MIT/BSD licensed. Does not support animated GIF resizing and has limited options for configuration. It is supported on a wide range of platforms but may not be supported in some Linux configurations without a custom build of the native libraries.
- [Cofoundry.Plugins.Imaging.ImageSharp](https://github.com/cofoundry-cms/Cofoundry.Plugins.Imaging.ImageSharp): Uses the [ImageSharp](https://github.com/SixLabors/ImageSharp) library which is licensed under the [Six Labors Split License](https://github.com/SixLabors/ImageSharp/blob/main/LICENSE). It is fully cross-platform, supports a wide range of formats including animated GIFs and has a comprehensive range of configuration options.

## Image File Location

By default Cofoundry will save image files into the "App_Data" folder inside your project. See the [File Storage docs](/framework/data-access/file-storage) for more information on configuring file storage options.

## Generating Image URLs

### From a View or Template

The [Cofoundry View Helper](Cofoundry-View-Helper) is the best way to access this:

```html
@using Cofoundry.Web

@model MyContentDisplayModel
@inject ICofoundryHelper<MyContentDisplayModel> Cofoundry

/* From a model */
<img src="@Cofoundry.Routing.ImageAsset(Model.ThumbnailImageAsset)">

/* From an id (see warning below) */
<img src="@await Cofoundry.Routing.ImageAssetAsync(3)">

/* Simple Resizing */
<img src="@Cofoundry.Routing.ImageAsset(Model.ThumbnailImageAsset, 200, 200)">

/* Resizing by passing in a constant of type IImageResizeSettings */
<img src="@Cofoundry.Routing.ImageAsset(Model.ThumbnailImageAsset, MyImageSizes.Thumbnail)">
```

Warning: Generating a URL from just an image asset id involves getting more information about the image from the database. Although caching is used to speed this up, it is recommended that you try to include the full `ImageAssetRenderDetails` object in your view model to take advantage of batch requests and async methods.

### From Code

You can request `IImageAssetRouteLibrary` from the DI container and use this to generate urls. It is the same API used by the Cofoundry View Helper above.

```csharp
using Cofoundry.Domain;

public class ImageExample
{
    private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;

    public ImageExample(IImageAssetRouteLibrary imageAssetRouteLibrary)
    {
        _imageAssetRouteLibrary = imageAssetRouteLibrary;
    }

    public string GetExampleUrl(IImageAssetRenderable image)
    {
        var url = _imageAssetRouteLibrary.ImageAsset(image, 200, 200);

        return url;
    }
}
```

## Getting Image Data

The simplest way to get image data is by resolving an instance of `IContentRepositry` from the DI container.

```csharp
using Cofoundry.Domain;

public class ImageExample
{
    private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;
    private readonly IContentRepository _contentRepository;

    public ImageExample(
        IImageAssetRouteLibrary imageAssetRouteLibrary,
        IContentRepository contentRepository
        )
    {
        _imageAssetRouteLibrary = imageAssetRouteLibrary;
        _contentRepository = contentRepository;
    }

    public async Task<string> GetExampleUrl(int imageId)
    {
        var image = await _contentRepository
            .ImageAssets()
            .GetById(imageId)
            .AsRenderDetails()
            .ExecuteAsync();
            
        var url = _imageAssetRouteLibrary.ImageAsset(image, 400, 600);

        return url;
    }
}
```