using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;
using Cofoundry.Domain;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    public static class AssetRouteLibrary
    {
        #region images

        public static string ImageAsset(IImageAssetRenderable asset, IImageResizeSettings settings = null)
        {
            if (asset == null) return string.Empty;
            SetDefaultCrop(asset, settings);

            return ImageAsset(asset.ImageAssetId, asset.FileName, asset.Extension, settings);
        }

        public static string ImageAsset(IImageAssetRenderable asset, int? width, int? height = null)
        {
            if (asset == null) return string.Empty;

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

            return ImageAsset(asset.ImageAssetId, asset.FileName, asset.Extension, settings);
        }

        private static string ImageAsset(int assetId, string fileName, string extension, IImageResizeSettings settings = null)
        {
            var pathFileName = Path.ChangeExtension(assetId + "_" + SlugFormatter.ToSlug(fileName), extension);
            var url = "/assets/images/" + pathFileName;
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
                if (settings.Mode == ImageFitMode.None)
                {
                    settings.Mode = ImageFitMode.Crop;
                }

                if (asset.DefaultAnchorLocation.HasValue)
                {
                    settings.Anchor = asset.DefaultAnchorLocation.Value;
                }
            }
        }

        private static bool isDefinedAndChanged(int settingValue, int imageValue) {
            return settingValue > 0 && settingValue != imageValue;
        }

        #endregion

        #region documents

        public static string DocumentAsset(IDocumentAssetRenderable asset)
        {
            if (asset == null) return string.Empty;

            return DocumentAsset(asset.DocumentAssetId, asset.FileName, asset.FileExtension);
        }

        private static string DocumentAsset(int assetId, string fileName, string extension)
        {
            var fn = Path.ChangeExtension(assetId + "_" + SlugFormatter.ToSlug(fileName), extension);
            var url = "/assets/files/" + fn;

            return url;
        }

        #endregion
    }
}