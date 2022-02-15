using Cofoundry.Core.Configuration;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Settings for ConfigBasedSiteUrlResolver.
    /// </summary>
    public class SiteUrlResolverSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// The root URL to use when resolving a relative to absolute 
        /// url, e.g. 'http://www.cofoundry.org'. If this value is not defined
        /// then the default implementation will fall back to using the URL from
        /// the request.
        /// </summary>
        public string SiteUrlRoot { get; set; }
    }
}
