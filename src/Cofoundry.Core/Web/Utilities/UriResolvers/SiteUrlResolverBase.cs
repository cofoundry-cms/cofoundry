using System;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// Base class for an ISiteUrlResolver that includes most of the bare functionality
    /// appart from working out the root url itself.
    /// </summary>
    public abstract class SiteUrlResolverBase :  ISiteUrlResolver
    {
        #region public methods

        /// <summary>
        /// Maps a relative path to an absolute path.
        /// </summary>
        /// <param name="path">Path to resolve.</param>
        /// <returns>The absolute path, or an empty string if the supplied path is null or empty.</returns>
        public string MakeAbsolute(string path)
        {
            return MakeAbsolute(path, false);
        }

        /// <summary>
        /// Maps a relative path to an absolute path
        /// </summary>
        /// <param name="path">path to resolve</param>
        /// <param name="forceSsl">whether to make the new uri https. If this is false then the scheme is not modified</param>
        /// <returns>The absolute path, or an empty string if the supplied path is null or empty.</returns>
        public string MakeAbsolute(string path, bool forceSsl)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;

            var trimmedPath = path.Trim();
            if (trimmedPath.StartsWith("~/")) trimmedPath = trimmedPath.Substring(1);
            if (!trimmedPath.StartsWith("/")) return MakeSsl(trimmedPath, forceSsl);
            string absolutePath = CleanPath(GetSiteRoot()) + trimmedPath.Substring(1);

            return MakeSsl(absolutePath, forceSsl);
        }

        #endregion

        #region private helpers
        
        protected abstract string GetSiteRoot();

        private string MakeSsl(string path, bool forceSsl)
        {
            if (forceSsl && path != null && path.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Insert(4, "s");
            }

            return path;
        }

        private string CleanPath(string siteRoot)
        {
            if (string.IsNullOrWhiteSpace(siteRoot)) throw new ArgumentNullException(siteRoot);

            siteRoot = siteRoot.Trim();
            if (!siteRoot.EndsWith("/"))
            {
                siteRoot += "/";
            }
            return siteRoot;
        }

        #endregion
    }
}
