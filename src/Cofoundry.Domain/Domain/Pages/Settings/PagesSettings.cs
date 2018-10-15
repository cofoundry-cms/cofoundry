using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Settings for the pages feature of Cofoundry.
    /// </summary>
    public class PagesSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// Disables the pages functionality, removing it from the admin
        /// panel and skipping registration of the dynamic page route. Access
        /// to pages is still possible from code if you choose to use those
        /// APIs from a user account with permissions.
        /// </summary>
        public bool Disabled { get; set; }
    }
}
