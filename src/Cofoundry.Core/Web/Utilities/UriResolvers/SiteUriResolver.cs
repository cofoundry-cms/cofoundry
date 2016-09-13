using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// A Uri resolver that relies on a configuration setting to construstr a uri.
    /// </summary>
    public class SiteUriResolver : SiteUriResolverBase
    {
        #region constructors

        private readonly SiteUriResolverSettings _siteUriResolverSettings;

        public SiteUriResolver(
            SiteUriResolverSettings siteUriResolverSettings
            )
        {
            _siteUriResolverSettings = siteUriResolverSettings;
        }

        #endregion

        protected override string GetSiteRoot()
        {
            return _siteUriResolverSettings.SiteUriRoot;
        }
    }
}
