using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Settings for the image assets feature of Cofoundry.
    /// </summary>
    public class ImageAssetsSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// Disables image asset functionality, removing it from the admin
        /// panel and skipping registration of image asset routes. Access
        /// to images is still possible from code if you choose to use those
        /// APIs from a user account with permissions.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Enables image asset routes that work for urls generated prior to
        /// v0.4 of Cofoundry. It isn't recommended to enable these unless you 
        /// really need to because the old routes were vulnerable to enumeration.
        /// </summary>
        public bool EnableCompatibilityRoutesFor0_4 { get; set; }

        /// <summary>
        /// Indicates whether dynamic image resizing should be disabled. Defaults to 
        /// false. An exception will be thrown if image resizing is requested but not
        /// enabled.
        /// </summary>
        /// <remakrs>
        /// If we were to return non-resized images instead of throwing an exception
        /// you could get problems further down the line with caching of incorrectly
        /// sized images.
        /// </remakrs>
        public bool DisableResizing { get; set; }

        /// <summary>
        /// The maximum size in pixels of the image that can be uploaded. Defaults
        /// to 3200. If uploading via the admin panel, oversized images will be resized
        /// automatically before uploading.
        /// </summary>
        public int MaxUploadWidth { get; set; } = 3200;

        /// <summary>
        /// The maximum height in pixels of the image that can be uploaded. Defaults
        /// to 3200. If uploading via the admin panel, oversized images will be resized
        /// automatically before uploading.
        /// </summary>
        public int MaxUploadHeight { get; set; } = 3200;

        /// <summary>
        /// The maximum width in pixels of that an image is permitted to be resized to.
        /// Defaults to 3200.
        /// </summary>
        public int MaxResizeWidth { get; set; } = 3200;

        /// <summary>
        /// The maximum height in pixels of that an image is permitted to be resized to.
        /// Defaults to 3200.
        /// </summary>
        public int MaxResizeHeight { get; set; } = 3200;

        /// <summary>
        /// The default max-age to use for the cache control header, measured in
        /// seconds. Image asset file urls are designed to be permanently cachable
        /// so the default value is 1 year.
        /// </summary>
        public int CacheMaxAge { get; set; } = 31536000;
    }
}
