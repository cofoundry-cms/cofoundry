using System.Diagnostics;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// SkiaSharp implementation of <see cref="IResizedImageAssetFileService"/>.
/// </summary>
public class SkiaSharpResizedImageAssetFileService : IResizedImageAssetFileService
{
    internal const string IMAGE_ASSET_CACHE_CONTAINER_NAME = "ImageAssetCache";
    private const string GIF_FILE_EXTENSION = "gif";

    private readonly IFileStoreService _fileService;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ISkiaSharpResizeSettingsValidator _skiaSharpResizeSettingsValidator;
    private readonly IResizeSpecificationFactory _resizeSpecificationFactory;
    private readonly ISkiaSharpImageResizer _skiaSharpImageResizer;
    private readonly SkiaSharpSettings _skiaSharpSettings;
    private readonly ILogger<SkiaSharpResizedImageAssetFileService> _logger;

    public SkiaSharpResizedImageAssetFileService(
        IFileStoreService fileService,
        IQueryExecutor queryExecutor,
        ISkiaSharpResizeSettingsValidator skiaSharpResizeSettingsValidator,
        IResizeSpecificationFactory resizeSpecificationFactory,
        ISkiaSharpImageResizer skiaSharpImageResizer,
        SkiaSharpSettings skiaSharpSettings,
        ILogger<SkiaSharpResizedImageAssetFileService> logger
        )
    {
        _fileService = fileService;
        _queryExecutor = queryExecutor;
        _logger = logger;
        _skiaSharpResizeSettingsValidator = skiaSharpResizeSettingsValidator;
        _resizeSpecificationFactory = resizeSpecificationFactory;
        _skiaSharpImageResizer = skiaSharpImageResizer;
        _skiaSharpSettings = skiaSharpSettings;
    }

    /// <inheritdoc/>
    public async Task<Stream> GetAsync(IImageAssetRenderable asset, IImageResizeSettings resizeSettings)
    {
        _skiaSharpResizeSettingsValidator.Validate(resizeSettings, asset);

        // Saving gifs is not supported by SkiaSharp, so they will either be reencoded to another 
        // format on upload or left as unresizeable.
        if (!resizeSettings.RequiresResizing(asset) || (asset.FileExtension == GIF_FILE_EXTENSION))
        {
            return await GetFileStreamAsync(asset.ImageAssetId);
        }

        var fullFileName = CreateCacheFilePathAndName(resizeSettings, asset);

        if (!_skiaSharpSettings.DisableFileCache && await _fileService.ExistsAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName))
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
            Stream outputStream;

            using (var imageSource = await LoadImageSource(asset))
            {
                if (!imageSource.IsLoaded)
                {
                    throw new InvalidOperationException($"{nameof(imageSource)} is expected to be loaded.");
                }

                var resizeSpecification = _resizeSpecificationFactory.Create(imageSource.Codec, imageSource.Bitmap, resizeSettings);
                var resizedImage = _skiaSharpImageResizer.Resize(imageSource.Bitmap, resizeSpecification);

                // Skia only supports a quality setting for Jpeg
                var data = resizedImage.Encode(imageSource.Codec.EncodedFormat, _skiaSharpSettings.JpegQuality);

                if (data == null)
                {
                    // e.g. this happens if trying to encode an image to a gif because gifs aren't supported.
                    throw new Exception($"Error encoding image asset {asset.ImageAssetId} to {imageSource.Codec.EncodedFormat}");
                }

                outputStream = data.AsStream(true);
            }

            await WriteImageToFileCacheAsync(fullFileName, outputStream);

            outputStream.Position = 0;
            return outputStream;
        }
    }

    /// <inheritdoc/>
    public Task ClearAsync(string fileNameOnDisk)
    {
        return _fileService.DeleteDirectoryAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fileNameOnDisk);
    }

    private async Task WriteImageToFileCacheAsync(string fullFileName, Stream outputStream)
    {
        if (!_skiaSharpSettings.DisableFileCache)
        {
            try
            {
                // Try and create the cache file, but don't throw an error if it fails - it will be attempted again on the next request
                outputStream.Position = 0;
                await _fileService.CreateIfNotExistsAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName, outputStream);
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
        }
    }

    private async Task<ImageFileSource> LoadImageSource(IImageAssetRenderable imageAsset)
    {
        var fileStream = await GetFileStreamAsync(imageAsset.ImageAssetId);
        var result = ImageFileSource.Load(fileStream);

        if (!result.IsLoaded)
        {
            result.Dispose();
            throw new Exception($"Unable to load image asset {imageAsset.ImageAssetId}");
        }

        return result;
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

    private static string CreateCacheFilePathAndName(IImageResizeSettings settings, IImageAssetRenderable asset)
    {
        var fileName = settings.CreateCacheFileName();
        var fileNameWithExtension = Path.ChangeExtension(fileName, asset.FileExtension);

        return asset.FileNameOnDisk + "/" + fileNameWithExtension;
    }
}
