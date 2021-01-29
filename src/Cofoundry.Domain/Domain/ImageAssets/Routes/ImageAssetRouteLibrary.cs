using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Route library for image assets
    /// </summary>
    public class ImageAssetRouteLibrary : IImageAssetRouteLibrary
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public ImageAssetRouteLibrary(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Simple but less efficient way of getting an image url if you only know 
        /// the id. Use the overload accepting an IImageAssetRenderable if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to get the url for</param>
        /// <param name="settings">Optional resizing settings for the image</param>
        public async Task<string> ImageAssetAsync(int? imageAssetId, IImageResizeSettings settings = null)
        {
            if (!imageAssetId.HasValue) return string.Empty;

            var getImageQuery = new GetImageAssetRenderDetailsByIdQuery(imageAssetId.Value);
            var asset = await _queryExecutor.ExecuteAsync(getImageQuery);

            return ImageAsset(asset, settings);
        }

        /// <summary>
        /// Simple but less efficient way of getting an image url if you only know 
        /// the id. Use the overload accepting an IImageAssetRenderable if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to get the url for</param>
        /// <param name="width">width to resize the image to</param>
        /// <param name="height">height to resize the image to</param>
        public async Task<string> ImageAssetAsync(int? imageAssetId, int? width, int? height = null)
        {
            if (!imageAssetId.HasValue) return string.Empty;

            var getImageQuery = new GetImageAssetRenderDetailsByIdQuery(imageAssetId.Value);
            var asset = await _queryExecutor.ExecuteAsync(getImageQuery);
            if (asset == null) return string.Empty;

            var settings = GetResizeSettings(asset, width, height);

            return GetUrl(asset, settings);
        }

        /// <summary>
        /// Gets the url for an image asset, with optional resizing parameters
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        /// <param name="settings">Optional resizing settings for the image</param>
        public string ImageAsset(IImageAssetRenderable asset, IImageResizeSettings settings = null)
        {
            if (asset == null) return string.Empty;
            SetDefaultCrop(asset, settings);

            return GetUrl(asset, settings);
        }

        /// <summary>
        /// Gets the url for an image asset, with optional resizing parameters
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        /// <param name="width">width to resize the image to</param>
        /// <param name="height">height to resize the image to</param>
        public string ImageAsset(IImageAssetRenderable asset, int? width, int? height = null)
        {
            if (asset == null) return string.Empty;
            var settings = GetResizeSettings(asset, width, height);

            return GetUrl(asset, settings);
        }

        #endregion

        #region private helpers

        private static ImageResizeSettings GetResizeSettings(IImageAssetRenderable asset, int? width, int? height)
        {
            var settings = new ImageResizeSettings();

            if (width.HasValue)
            {
                settings.Width = width.Value;
            }

            if (height.HasValue)
            {
                settings.Height = height.Value;
            }

            SetDefaultCrop(asset, settings);

            return settings;
        }

        private static string GetUrl(IImageAssetRenderable asset, IImageResizeSettings settings = null)
        {
            var fileName = Path.ChangeExtension(SlugFormatter.ToSlug(asset.FileName), asset.FileExtension);
            var pathPart = asset.ImageAssetId + "-" + asset.FileStamp + "-" + asset.VerificationToken;
            var url = $"/assets/images/{pathPart}/{fileName}";

            if (settings != null)
            {
                url += settings.ToQueryString();
            }

            return url;
        }

        private static void SetDefaultCrop(IImageAssetRenderable asset, IImageResizeSettings settings)
        {
            if (settings == null) return;

            if (isDefinedAndChanged(settings.Width, asset.Width) || isDefinedAndChanged(settings.Height, asset.Height))
            {
                if (asset.DefaultAnchorLocation.HasValue)
                {
                    settings.Anchor = asset.DefaultAnchorLocation.Value;
                }
            }
        }

        private static bool isDefinedAndChanged(int settingValue, int imageValue)
        {
            return settingValue > 0 && settingValue != imageValue;
        }

        #endregion
    }
}
