using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to validate the resize settings sent from the client prior
    /// to resizing the image.
    /// </summary>
    public class ImageResizeSettingsValidator : IImageResizeSettingsValidator
    {
        private readonly ImageAssetsSettings _imageAssetsSettings;

        public ImageResizeSettingsValidator(
            ImageAssetsSettings imageAssetsSettings
            )
        {
            _imageAssetsSettings = imageAssetsSettings;
        }

        /// <summary>
        /// Validates that the image asset can be resized using the settings
        /// supplied by the client.
        /// </summary>
        /// <param name="settings">Settings to validate.</param>
        /// <param name="asset">Asset attempting to be resized. Cannot be null.</param>
        public void Validate(IImageResizeSettings settings, IImageAssetRenderable asset)
        {
            if (settings.RequiresResizing(asset) && _imageAssetsSettings.DisableResizing)
            {
                throw new InvalidImageResizeSettingsException("Image resizing has been requested but is disabled.", settings);
            }

            if (settings.Width > _imageAssetsSettings.MaxResizeWidth)
            {
                throw new InvalidImageResizeSettingsException($"Requested image width exceeds the maximum permitted value of {_imageAssetsSettings.MaxResizeWidth} pixels");
            }

            if (settings.Height > _imageAssetsSettings.MaxResizeHeight)
            {
                throw new InvalidImageResizeSettingsException($"Requested image height exceeds the maximum permitted value of {_imageAssetsSettings.MaxResizeHeight} pixels");
            }
        }
    }
}
