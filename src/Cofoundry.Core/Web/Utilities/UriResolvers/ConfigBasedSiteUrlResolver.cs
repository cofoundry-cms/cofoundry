using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// A Url resolver that relies on a configuration setting to construstr a url.
    /// </summary>
    public class ConfigBasedSiteUrlResolver : SiteUrlResolverBase
    {
        #region constructors

        private readonly SiteUrlResolverSettings _siteUrlResolverSettings;

        public ConfigBasedSiteUrlResolver(
            SiteUrlResolverSettings siteUriResolverSettings
            )
        {
            _siteUrlResolverSettings = siteUriResolverSettings;
        }

        #endregion

        /// <summary>
        /// Indicates whether we have valid setting and that
        /// we are able to resolve a url
        /// </summary>
        public bool CanResolve()
        {
            return !string.IsNullOrWhiteSpace(_siteUrlResolverSettings.SiteUrlRoot);
        }

        /// <summary>
        /// Maps a relative path to an absolute url e.g.
        /// /mypage.htm into http://www.mysite/mypage.htm
        /// </summary>
        /// <returns>The absolute path, or an empty string if the supplied path is null or empty.</returns>
        protected override string GetSiteRoot()
        {
            if (!CanResolve())
            {
                throw new InvalidOperationException("Cofoundry:SiteUrlResolver:SiteUrlRoot setting must be defined in order to resolve an absolute url outside of a web request.");
            }

            return _siteUrlResolverSettings.SiteUrlRoot;
        }
    }
}
