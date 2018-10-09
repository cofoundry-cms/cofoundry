using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Settings for the document assets feature of Cofoundry.
    /// </summary>
    public class DocumentAssetsSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// Disables document asset functionality, removing it from the admin
        /// panel and skipping registration of document asset routes. Access
        /// to document is still possible from code if you choose to use those
        /// APIs from a user account with permissions.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Enables document asset routes that work for urls generated prior to
        /// v0.4 of Cofoundry. It isn't recommended to enable these unless you 
        /// really need to because the old routes were vulnerable to enumeration.
        /// </summary>
        public bool EnableCompatibilityRoutesFor0_4 { get; set; }

        /// <summary>
        /// The default max-age to use for the cache control header, measured in
        /// seconds. Document asset file urls are designed to be permanently cachable
        /// so the default value is 1 year.
        /// </summary>
        public int CacheMaxAge { get; set; } = 31536000;
    }
}
