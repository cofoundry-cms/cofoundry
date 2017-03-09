using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// WebDirectories represent a folder in the dynamic web page hierarchy.
    /// This representation is used in dynamic page routing and is designed to
    /// be lightweight and cached.
    /// </summary>
    public class WebDirectoryRoute : IEquatable<WebDirectoryRoute>
    {
        /// <summary>
        /// Database id of the web directory.
        /// </summary>
        public int WebDirectoryId { get; set; }

        /// <summary>
        /// Id of the parent directory. This can only be null for the 
        /// root web directory.
        /// </summary>
        public int? ParentWebDirectoryId { get; set; }

        /// <summary>
        /// Url slug used to create a path for this directory. Should not
        /// contain any slashes, just alpha-numerical with dashes.
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// The complete path of up to and including
        /// this directory. Does not use a trailing slash.
        /// </summary>
        public string FullUrlPath { get; set; }

        /// <summary>
        /// Display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contains variations of this directory path based on specific locales. 
        /// </summary>
        /// <remarks>
        /// This functionality isn't really well supported in the admin UI, but is 
        /// supported in page routing. One2Many uses this style of directory routing for the /news folder
        /// but it would seem that the locales have been added in the db manually rather than through
        /// the user interface. In other sites a separate web directory is added for alternative locales which
        /// allows for more flexibility in regional variation in site structure. Supporting both might be confusing but
        /// may be required?
        /// </remarks>
        public IEnumerable<WebDirectoryRouteLocale> LocaleVariations { get; set; }

        #region public methods

        /// <summary>
        /// Determins if the specified path matches this directory, does not 
        /// attempt any locale matching.
        /// </summary>
        public bool MatchesPath(string path)
        {
            string trimmedPath = path.Trim('/');
            bool containsPath = FullUrlPath.Equals(trimmedPath, StringComparison.OrdinalIgnoreCase);

            return containsPath;
        }

        /// <summary>
        /// Determins if the specified path and locale matches this directory. If
        /// there isn't an applicable locale veriation, then the matching is done against the default path.
        /// </summary>
        public bool MatchesPath(string path, int localeId)
        {
            bool containsPath = false;

            var localePath = LocaleVariations.SingleOrDefault(l => l.LocaleId == localeId);
            if (localePath != null)
            {
                string trimmedPath = path.Trim('/');
                containsPath = localePath.FullUrlPath.Equals(trimmedPath, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // Use default directory
                containsPath = MatchesPath(path);
            }

            return containsPath;
        }

        public bool IsSiteRoot()
        {
            return !ParentWebDirectoryId.HasValue && FullUrlPath == "/";
        }

        #endregion

        #region equality

        public bool Equals(WebDirectoryRoute other)
        {
            if (other == null) return false;
            if (other == this) return true;

            return other.WebDirectoryId == WebDirectoryId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WebDirectoryRoute);
        }

        public override int GetHashCode()
        {
            return WebDirectoryId;
        }

        #endregion
    }
}
