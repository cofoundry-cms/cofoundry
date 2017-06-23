using Cofoundry.Core.ErrorLogging;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Processing;
using System.Diagnostics;
using System.Net;

namespace Cofoundry.Plugins.ImageResizing.ImageSharp
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
        private readonly IErrorLoggingService _errorLoggingService;

        #endregion

        #region constructor

        public ImageSharpResizedImageAssetFileService(
            IFileStoreService fileService,
            IQueryExecutor queryExecutor,
            IErrorLoggingService errorLoggingService
            )
        {
            _fileService = fileService;
            _queryExecutor = queryExecutor;
            _errorLoggingService = errorLoggingService;
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

            if (_fileService.Exists(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName))
            {
                return _fileService.Get(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName);
            }
            else
            {
                imageStream = new MemoryStream();

                using (var originalStream = await GetFileStreamAsync(asset.ImageAssetId))
                using (var image = Image.Load(originalStream))
                {
                    var resizeOptions = ConvertSettings(inputSettings);
                    image.Resize(resizeOptions);

                    if (!string.IsNullOrWhiteSpace(inputSettings.BackgroundColor))
                    {
                        var color = Rgba32.FromHex(inputSettings.BackgroundColor);
                        image.BackgroundColor(color);
                    }

                    image.Save(imageStream);
                }

                try
                {
                    // Try and create the cache file, but don't throw an error if it fails - it will be attempted again on the next request
                    _fileService.CreateIfNotExists(IMAGE_ASSET_CACHE_CONTAINER_NAME, fullFileName, imageStream);
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                    {
                        throw;
                    }
                    else
                    {
                        _errorLoggingService.Log(ex);
                    }
                }

                imageStream.Position = 0;
                return imageStream;
            }
        }

        public void Clear(int imageAssetId)
        {
            var directory = imageAssetId.ToString();
            _fileService.DeleteDirectory(IMAGE_ASSET_CACHE_CONTAINER_NAME, directory);
        }

        #region private methods

        private async Task<Stream> GetFileStreamAsync(int imageAssetId)
        {
            var result = await _queryExecutor.GetByIdAsync<ImageAssetFile>(imageAssetId);

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
                    resizeSettings.Position = AnchorPosition.Bottom;
                    break;
                case ImageAnchorLocation.BottomLeft:
                    resizeSettings.Position = AnchorPosition.BottomLeft;
                    break;
                case ImageAnchorLocation.BottomRight:
                    resizeSettings.Position = AnchorPosition.BottomRight;
                    break;
                case ImageAnchorLocation.MiddleCenter:
                    resizeSettings.Position = AnchorPosition.Center;
                    break;
                case ImageAnchorLocation.MiddleLeft:
                    resizeSettings.Position = AnchorPosition.Left;
                    break;
                case ImageAnchorLocation.MiddleRight:
                    resizeSettings.Position = AnchorPosition.Right;
                    break;
                case ImageAnchorLocation.TopCenter:
                    resizeSettings.Position = AnchorPosition.Top;
                    break;
                case ImageAnchorLocation.TopLeft:
                    resizeSettings.Position = AnchorPosition.TopLeft;
                    break;
                case ImageAnchorLocation.TopRight:
                    resizeSettings.Position = AnchorPosition.TopRight;
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
