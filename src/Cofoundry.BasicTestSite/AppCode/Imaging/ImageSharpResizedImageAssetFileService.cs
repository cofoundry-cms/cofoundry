using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Net;
using SixLabors.Primitives;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace Cofoundry.Plugins.Imaging.ImageSharp
{
    /// <summary>
    /// Service for resizing and caching the resulting image.
    /// </summary>
    public class ImageSharpResizedImageAssetFileService : IResizedImageAssetFileService
    {
        #region private member variables

        internal static readonly string IMAGE_ASSET_CACHE_CONTAINER_NAME = "ImageAssetCache";

        private const int MAX_IMG_SIZE = 3200;

        private readonly IFileStoreService _fileService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILogger<ImageSharpResizedImageAssetFileService> _logger;

        #endregion

        #region constructor

        public ImageSharpResizedImageAssetFileService(
            IFileStoreService fileService,
            IQueryExecutor queryExecutor,
            ILogger<ImageSharpResizedImageAssetFileService> logger
            )
        {
            _fileService = fileService;
            _queryExecutor = queryExecutor;
            _logger = logger;
        }

        #endregion

        public async Task<Stream> GetAsync(IImageAssetRenderable asset, IImageResizeSettings inputSettings)
        {
            if ((inputSettings.Width < 1 && inputSettings.Height < 1)
                || (inputSettings.Width == asset.Width && inputSettings.Height == asset.Height))
            {
                return await GetFileStreamAsync(asset.ImageAssetId);
            }
            
            var directory = asset.ImageAssetId.ToString();
            var fullFileName = directory + "/" + CreateCacheFileName(inputSettings, asset);
            Stream imageStream = null;
            ValidateSettings(inputSettings);

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
                    if (imageFormat == null) throw new Exception("Unable to determine image type for image asset " + asset.ImageAssetId);
                    var resizeOptions = ConvertSettings(inputSettings);
                    image.Mutate(cx => 
                    {
                        cx.Resize(resizeOptions);
                        
                        if (!string.IsNullOrWhiteSpace(inputSettings.BackgroundColor))
                        {
                            var color = Rgba32.FromHex(inputSettings.BackgroundColor);
                            cx.BackgroundColor(color);
                        }
                        else if (CanPad(resizeOptions) && !SupportsTransparency(imageFormat))
                        {
                            // default background for jpg encoder is black, but white is a better default in most scenarios.
                            cx.BackgroundColor(Rgba32.White);
                        }
                    });


                    image.Save(imageStream, imageFormat);
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

        private bool CanPad(ResizeOptions resizeOptions)
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

        public bool SupportsTransparency(IImageFormat imageFormat)
        {
            return imageFormat != ImageFormats.Jpeg && imageFormat != ImageFormats.Png;
        }

        public Task ClearAsync(int imageAssetId)
        {
            var directory = imageAssetId.ToString();
            return _fileService.DeleteDirectoryAsync(IMAGE_ASSET_CACHE_CONTAINER_NAME, directory);
        }

        #region private methods

        private async Task<Stream> GetFileStreamAsync(int imageAssetId)
        {
            var query = new GetImageAssetFileByIdQuery(imageAssetId);
            var result = await _queryExecutor.ExecuteAsync(query);

            if (result == null || result.ContentStream == null)
            {
                throw new FileNotFoundException(imageAssetId.ToString());
            }

            return result.ContentStream;
        }

        private ResizeOptions ConvertSettings(IImageResizeSettings inputSettings)
        {
            var resizeSettings = new ResizeOptions();
            resizeSettings.Size = new Size(inputSettings.Width, inputSettings.Height);

            // Scale options not supported yet
            //resizeSettings.Scale = (ScaleMode)inputSettings.Scale;

            switch (inputSettings.Mode)
            {
                case ImageFitMode.Default:
                case ImageFitMode.Crop:
                    resizeSettings.Mode = ResizeMode.Crop;
                    break;
                case ImageFitMode.Max:
                    resizeSettings.Mode = ResizeMode.Max;
                    break;
                case ImageFitMode.Pad:
                    resizeSettings.Mode = ResizeMode.Pad;
                    break;
                default:
                    throw new NotSupportedException("ImageFitMode not supported: " + inputSettings.Mode);
            }

            switch (inputSettings.Anchor)
            {
                case ImageAnchorLocation.BottomCenter:
                    resizeSettings.Position = AnchorPositionMode.Bottom;
                    break;
                case ImageAnchorLocation.BottomLeft:
                    resizeSettings.Position = AnchorPositionMode.BottomLeft;
                    break;
                case ImageAnchorLocation.BottomRight:
                    resizeSettings.Position = AnchorPositionMode.BottomRight;
                    break;
                case ImageAnchorLocation.MiddleCenter:
                    resizeSettings.Position = AnchorPositionMode.Center;
                    break;
                case ImageAnchorLocation.MiddleLeft:
                    resizeSettings.Position = AnchorPositionMode.Left;
                    break;
                case ImageAnchorLocation.MiddleRight:
                    resizeSettings.Position = AnchorPositionMode.Right;
                    break;
                case ImageAnchorLocation.TopCenter:
                    resizeSettings.Position = AnchorPositionMode.Top;
                    break;
                case ImageAnchorLocation.TopLeft:
                    resizeSettings.Position = AnchorPositionMode.TopLeft;
                    break;
                case ImageAnchorLocation.TopRight:
                    resizeSettings.Position = AnchorPositionMode.TopRight;
                    break;
                default:
                    throw new NotSupportedException("ImageAnchorLocation not supported: " + inputSettings.Anchor);
            }

            return resizeSettings;
        }

        private void ValidateSettings(IImageResizeSettings settings)
        {
            // Scale mode not supported yet
            settings.Scale = ImageScaleMode.Both;

            if (settings.Width > MAX_IMG_SIZE) settings.Width = MAX_IMG_SIZE;
            if (settings.Height > MAX_IMG_SIZE) settings.Height = MAX_IMG_SIZE;
        }

        private string CreateCacheFileName(IImageResizeSettings settings, IImageAssetRenderable asset)
        {
            const string format = "w{0}h{1}c{2}s{3}bg{4}a{5}";
            string s = string.Format(format, settings.Width, settings.Height, settings.Mode, settings.Scale, settings.BackgroundColor, settings.Anchor);
            s = WebUtility.UrlEncode(s);

            return Path.ChangeExtension(s, asset.Extension);
        }

        #endregion
    }
}
