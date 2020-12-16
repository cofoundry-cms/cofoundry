using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// This is the default url resolver that will attempt to resolve using configuration
    /// settings, but falls back to using the http request if it available.
    /// </summary>
    public class CompositeSiteUrlResolver : ISiteUrlResolver
    {
        private readonly ConfigBasedSiteUrlResolver _configBasedSiteUrlResolver;
        private readonly RequestBasedSiteUrlResolver _requestBasedSiteUrlResolver;

        public CompositeSiteUrlResolver(
            ConfigBasedSiteUrlResolver configBasedSiteUrlResolver,
            RequestBasedSiteUrlResolver requestBasedSiteUrlResolver
            )
        {
            _configBasedSiteUrlResolver = configBasedSiteUrlResolver;
            _requestBasedSiteUrlResolver = requestBasedSiteUrlResolver;
        }

        /// <summary>
        /// Maps a relative path to an absolute url e.g.
        /// /mypage.htm into http://www.mysite/mypage.htm
        /// </summary>
        /// <param name="path">Path to resolve</param>
        /// <returns>The absolute path, or an empty string if the supplied path is null or empty.</returns>
        public string MakeAbsolute(string path)
        {
            return GetResolver().MakeAbsolute(path);
        }

        /// <summary>
        /// Maps a relative path to an absolute path
        /// </summary>
        /// <param name="path">path to resolve</param>
        /// <param name="forceSsl">whether to make the new uri https. If this is false then the scheme is not modified</param>
        /// <returns>The absolute path, or an empty string if the supplied path is null or empty.</returns>
        public string MakeAbsolute(string path, bool forceSsl)
        {
            return GetResolver().MakeAbsolute(path, forceSsl);
        }

        private ISiteUrlResolver GetResolver()
        {
            if (_configBasedSiteUrlResolver.CanResolve())
            {
                return _configBasedSiteUrlResolver;
            }
            else if (_requestBasedSiteUrlResolver.CanResolve())
            {
                return _requestBasedSiteUrlResolver;
            }

            // Default to the config error message if we're outside a request
            return _configBasedSiteUrlResolver;
        }
    }
}
