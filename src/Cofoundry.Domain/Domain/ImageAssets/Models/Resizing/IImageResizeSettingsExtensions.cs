using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Cofoundry.Domain
{
    public static class IImageResizeSettingsExtensions
    {
        /// <summary>
        /// Determines if the settings indicate that an image asset should 
        /// be resized. If width and hight are not specified or the dimensions
        /// in the settings are the same as the image then false will be returned.
        /// </summary>
        /// <param name="asset">The asset to check. Cannot be null.</param>
        public static bool RequiresResizing(this IImageResizeSettings settings, IImageAssetRenderable asset)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            if ((settings.Width < 1 && settings.Height < 1)
                || (settings.Width == asset.Width && settings.Height == asset.Height))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Indicates if the resize width has been specified i.e. is greater than 0.
        /// </summary>
        public static bool IsWidthSet(this IImageResizeSettings settings)
        {
            return settings.Width > 0;
        }

        /// <summary>
        /// Indicates if the resize height has been specified i.e. is greater than 0.
        /// </summary>
        public static bool IsHeightSet(this IImageResizeSettings settings)
        {
            return settings.Height > 0;
        }

        /// <summary>
        /// Creates a short string unique to the settings which can be used
        /// as a file name.
        /// </summary>
        public static string CreateCacheFileName(this IImageResizeSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            const string format = "w{0}h{1}c{2}s{3}bg{4}a{5}";
            string s = string.Format(format, settings.Width, settings.Height, settings.Mode, settings.Scale, settings.BackgroundColor, settings.Anchor);
            s = WebUtility.UrlEncode(s);

            return s;
        }
    }
}
