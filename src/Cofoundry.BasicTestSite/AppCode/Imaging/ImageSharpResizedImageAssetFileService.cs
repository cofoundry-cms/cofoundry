using System.Diagnostics;
using System.Globalization;
using System.Net;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Cofoundry.Plugins.Imaging.ImageSharp;

/// <summary>
/// Service for resizing and caching the resulting image.
/// </summary>
public class ImageSharpResizedImageAssetFileService : IResizedImageAssetFileService
{
    internal const string IMAGE_ASSET_CACHE_CONTAINER_NAME = "ImageAssetCache";

    private readonly IFileStoreService _fileService;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ImageAssetsSettings _imageAssetsSettings;
    private readonly ILogger<ImageSharpResizedImageAssetFileService> _logger;

    public ImageSharpResizedImageAssetFileService(
        IFileStoreService fileService,
        IQueryExecutor queryExecutor,
        ImageAssetsSettings imageAssetsSettings,
        ILogger<ImageSharpResizedImageAssetFileService> logger
        )
    {
        _fileService = fileService;
        _queryExecutor = queryExecutor;
        _logger = logger;
        _imageAssetsSettings = imageAssetsSettings;
    }

    public async Task<Stream> GetAsync(IImageAssetRenderable asset, IImageResizeSettings settings)
    {
        if ((settings.Width < 1 && settings.Height < 1)
            || (settings.Width == asset.Width && settings.Height == asset.Height))
        {
            return await GetFileStreamAsync(asset.ImageAssetId);
        }

        if (_imageAssetsSettings.DisableResizing)
        {
            throw new InvalidImageResizeSettingsException("Image resizing has been requested but is disabled.", settings);
        }

        var fullFileName = asset.FileNameOnDisk + "/" + CreateCacheFileName(settings, asset);
        Stream imageStream = null;
        ValidateSettings(settings);

        if (await _fileService.ExistsAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName))
        {
            return await _fileService.GetAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName);
        }
        else
        {
            imageStream = new MemoryStream();
            IImageFormat imageFormat = null;
            using (var originalStream = await GetFileStreamAsync(asset.ImageAssetId))
            using (var image = Image.Load(originalStream, out imageFormat))
            {
                if (imageFormat == null)
                {
                    throw new Exception("Unable to determine image type for image asset " + asset.ImageAssetId);
                }

                var encoder = Configuration.Default.ImageFormatsManager.FindEncoder(imageFormat);
                if (encoder == null)
                {
                    throw new InvalidOperationException("Encoder not found for image format " + imageFormat.Name);
                }

                var resizeOptions = ConvertSettings(settings);
                image.Mutate(cx =>
                {
                    cx.Resize(resizeOptions);

                    if (!string.IsNullOrWhiteSpace(settings.BackgroundColor))
                    {
                        var color = Rgba32.ParseHex(settings.BackgroundColor);
                        cx.BackgroundColor(color);
                    }
                    else if (CanPad(resizeOptions))
                    {
                        if (SupportsTransparency(imageFormat))
                        {
                            cx.BackgroundColor(Color.Transparent);
                            encoder = EnsureEncoderSupportsTransparency(encoder);
                        }
                        else
                        {
                            // default background for jpg encoder is black, but white is a better default in most scenarios.
                            cx.BackgroundColor(Color.White);
                        }
                    }
                });

                image.Save(imageStream, encoder);
            }

            try
            {
                // Try and create the cache file, but don't throw an error if it fails - it will be attempted again on the next request
                await _fileService.CreateIfNotExistsAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName, imageStream);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    throw;
                }
                else
                {
                    _logger.LogError(0, ex, "Error creating image asset cache file. Container name {ContainerName}, {fullFileName}", IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName);
                }
            }

            imageStream.Position = 0;
            return imageStream;
        }
    }

    public bool SupportsTransparency(IImageFormat imageFormat)
    {
        return imageFormat != JpegFormat.Instance && imageFormat != BmpFormat.Instance;
    }

    public Task ClearAsync(string fileNameOnDisk)
    {
        return _fileService.DeleteDirectoryAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fileNameOnDisk);
    }

    private async Task<Stream> GetFileStreamAsync(int imageAssetId)
    {
        var query = new GetImageAssetFileByIdQuery(imageAssetId);
        var result = await _queryExecutor.ExecuteAsync(query);

        if (result == null || result.ContentStream == null)
        {
            throw new FileNotFoundException(imageAssetId.ToString(CultureInfo.InvariantCulture));
        }

        return result.ContentStream;
    }

    private static ResizeOptions ConvertSettings(IImageResizeSettings inputSettings)
    {
        var resizeSettings = new ResizeOptions();
        resizeSettings.Size = new Size(inputSettings.Width, inputSettings.Height);

        // Scale options not supported yet
        //resizeSettings.Scale = (ScaleMode)inputSettings.Scale;

        resizeSettings.Mode = inputSettings.Mode switch
        {
            ImageFitMode.Default or ImageFitMode.Crop => ResizeMode.Crop,
            ImageFitMode.Max => ResizeMode.Max,
            ImageFitMode.Pad => ResizeMode.Pad,
            _ => throw new NotSupportedException("ImageFitMode not supported: " + inputSettings.Mode),
        };

        resizeSettings.Position = inputSettings.Anchor switch
        {
            ImageAnchorLocation.BottomCenter => AnchorPositionMode.Bottom,
            ImageAnchorLocation.BottomLeft => AnchorPositionMode.BottomLeft,
            ImageAnchorLocation.BottomRight => AnchorPositionMode.BottomRight,
            ImageAnchorLocation.MiddleCenter => AnchorPositionMode.Center,
            ImageAnchorLocation.MiddleLeft => AnchorPositionMode.Left,
            ImageAnchorLocation.MiddleRight => AnchorPositionMode.Right,
            ImageAnchorLocation.TopCenter => AnchorPositionMode.Top,
            ImageAnchorLocation.TopLeft => AnchorPositionMode.TopLeft,
            ImageAnchorLocation.TopRight => AnchorPositionMode.TopRight,
            _ => throw new NotSupportedException("ImageAnchorLocation not supported: " + inputSettings.Anchor),
        };

        return resizeSettings;
    }

    private static IImageEncoder EnsureEncoderSupportsTransparency(IImageEncoder encoder)
    {
        if (encoder is PngEncoder pngEncoder)
        {
            if (pngEncoder.ColorType != PngColorType.RgbWithAlpha)
            {
                encoder = new PngEncoder()
                {
                    ColorType = PngColorType.RgbWithAlpha,
                    CompressionLevel = pngEncoder.CompressionLevel,
                    FilterMethod = pngEncoder.FilterMethod,
                    Gamma = pngEncoder.Gamma,
                    InterlaceMethod = pngEncoder.InterlaceMethod,
                    Quantizer = pngEncoder.Quantizer,
                    TextCompressionThreshold = pngEncoder.TextCompressionThreshold,
                    Threshold = pngEncoder.Threshold
                };
            }
        }

        return encoder;
    }

    private static bool CanPad(ResizeOptions resizeOptions)
    {
        switch (resizeOptions.Mode)
        {
            case ResizeMode.Crop:
            case ResizeMode.Max:
            case ResizeMode.Min:
            case ResizeMode.Stretch:
                return false;
        }

        return true;
    }

    private void ValidateSettings(IImageResizeSettings settings)
    {
        // Scale mode not supported yet
        settings.Scale = ImageScaleMode.Both;

        if (settings.Width > _imageAssetsSettings.MaxResizeWidth)
        {
            throw new InvalidImageResizeSettingsException($"Requested image width exceeds the maximum permitted value of {_imageAssetsSettings.MaxResizeWidth} pixels");
        }

        if (settings.Height > _imageAssetsSettings.MaxResizeHeight)
        {
            throw new InvalidImageResizeSettingsException($"Requested image height exceeds the maximum permitted value of {_imageAssetsSettings.MaxResizeHeight} pixels");
        }
    }

    private static string CreateCacheFileName(IImageResizeSettings settings, IImageAssetRenderable asset)
    {
        var fileName = $"w{settings.Width}h{settings.Height}c{settings.Mode}s{settings.Scale}bg{settings.BackgroundColor}a{settings.Anchor}";
        fileName = WebUtility.UrlEncode(fileName);

        return Path.ChangeExtension(fileName, asset.FileExtension);
    }
}
