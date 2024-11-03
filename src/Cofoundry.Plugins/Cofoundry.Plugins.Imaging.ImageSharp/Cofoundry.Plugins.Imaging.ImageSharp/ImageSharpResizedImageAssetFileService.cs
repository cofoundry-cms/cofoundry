using System.Net;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Cofoundry.Plugins.Imaging.ImageSharp;

/// <summary>
/// ImageSharp implementation of <see cref="IResizedImageAssetFileService"/>.
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

    /// <inheritdoc/>
    public async Task<Stream> GetAsync(IImageAssetRenderable asset, IImageResizeSettings inputSettings)
    {
        if ((inputSettings.Width < 1 && inputSettings.Height < 1)
            || (inputSettings.Width == asset.Width && inputSettings.Height == asset.Height))
        {
            return await GetFileStreamAsync(asset.ImageAssetId);
        }

        if (_imageAssetsSettings.DisableResizing)
        {
            throw new InvalidImageResizeSettingsException("Image resizing has been requested but is disabled.", inputSettings);
        }

        var fullFileName = asset.FileNameOnDisk + "/" + CreateCacheFileName(inputSettings, asset);
        Stream? imageStream = null;
        ValidateSettings(inputSettings);

        if (await _fileService.ExistsAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName))
        {
            var stream = await _fileService.GetAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName);
            if (stream == null)
            {
                throw new FileNotFoundException($"Cache file at {nameof(fullFileName)} existed, but could not be read.");
            }

            return stream;
        }
        else
        {
            imageStream = new MemoryStream();
            using (var originalStream = await GetFileStreamAsync(asset.ImageAssetId))
            {
                using (var image = Image.Load(originalStream))
                {
                    var imageFormat = image.Metadata.DecodedImageFormat;
                    if (imageFormat == null)
                    {
                        throw new Exception("Unable to determine image type for image asset " + asset.ImageAssetId);
                    }

                    var encoder = Configuration.Default.ImageFormatsManager.GetEncoder(imageFormat);
                    if (encoder == null)
                    {
                        throw new InvalidOperationException("Encoder not found for image format " + imageFormat.Name);
                    }

                    var resizeOptions = ConvertSettings(inputSettings);
                    image.Mutate(cx =>
                    {
                        cx.Resize(resizeOptions);

                        if (!string.IsNullOrWhiteSpace(inputSettings.BackgroundColor))
                        {
                            var color = Rgba32.ParseHex(inputSettings.BackgroundColor);
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
            }

            try
            {
                // Try and create the cache file, but don't throw an error if it fails - it will be attempted again on the next request
                await _fileService.CreateIfNotExistsAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName, imageStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error creating image asset cache file. Container name {ContainerName}, {fullFileName}", IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName);
            }

            imageStream.Position = 0;
            return imageStream;
        }
    }

    /// <inheritdoc/>
    public Task ClearAsync(string fileNameOnDisk)
    {
        return _fileService.DeleteDirectoryAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fileNameOnDisk);
    }

    private static bool SupportsTransparency(IImageFormat imageFormat)
    {
        return imageFormat != JpegFormat.Instance && imageFormat != BmpFormat.Instance;
    }

    private async Task<Stream> GetFileStreamAsync(int imageAssetId)
    {
        var query = new GetImageAssetFileByIdQuery(imageAssetId);
        var result = await _queryExecutor.ExecuteAsync(query);

        if (result == null || result.ContentStream == null)
        {
            throw new FileNotFoundException($"Could not find file for ImageAssetId {imageAssetId}");
        }

        return result.ContentStream;
    }

    private static ResizeOptions ConvertSettings(IImageResizeSettings inputSettings)
    {
        var resizeSettings = new ResizeOptions();
        resizeSettings.Size = new(inputSettings.Width, inputSettings.Height);

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
        return resizeOptions.Mode switch
        {
            ResizeMode.Crop or ResizeMode.Max or ResizeMode.Min or ResizeMode.Stretch => false,
            _ => true,
        };
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
