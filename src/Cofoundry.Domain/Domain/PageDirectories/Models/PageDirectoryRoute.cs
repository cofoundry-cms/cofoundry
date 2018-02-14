using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// PageDirectories represent a folder in the dynamic web page hierarchy.
    /// This representation is used in dynamic page routing and is designed to
    /// be lightweight and cached.
    /// </summary>
    public class PageDirectoryRoute : IEquatable<PageDirectoryRoute>
    {
        /// <summary>
        /// Database id of the page directory.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// Id of the parent directory. This can only be null for the 
        /// root page directory.
        /// </summary>
        public int? ParentPageDirectoryId { get; set; }

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
        /// supported in page routing. It was originally introduced many years before 
        /// cofoundry was open sourced and appeared to have been managed in the db manually 
        /// rather than through the user interface. More often a separate page directory is added 
        /// for alternative locales which allows for more flexibility in regional variation in site 
        /// structure. Supporting both might be confusing but the requirements need to be more fully 
        /// explored.
        /// </remarks>
        public ICollection<PageDirectoryRouteLocale> LocaleVariations { get; set; }

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
            return !ParentPageDirectoryId.HasValue && FullUrlPath == "/";
        }

        #endregion

        #region equality

        public bool Equals(PageDirectoryRoute other)
        {
            if (other == null) return false;
            if (other == this) return true;

            return other.PageDirectoryId == PageDirectoryId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PageDirectoryRoute);
        }

        public override int GetHashCode()
        {
            return PageDirectoryId;
        }

        #endregion
    }
}
