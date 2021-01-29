using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class PagePathHelper : IPagePathHelper
    {
        const string PATH_DELIMITER = "/";

        /// <summary>
        /// Standardizes a page path ensuring it 
        /// - starts with a slash
        /// - doesn't end with a trailing slash
        /// </summary>
        /// <param name="path">Path to standardize.</param>
        public string StandardizePath(string path)
        {
            return StandardizePathWithoutLocale(path, null);
        }

        /// <summary>
        /// Standardizes a page path ensuring it 
        /// - starts with a slash
        /// - doesn't end with a trailing slash
        /// - (optionally) does not contain the locale
        /// </summary>
        /// <param name="path">Path to standardize.</param>
        /// <param name="currentLocale">Locale of the path to remove if present.</param>
        public string StandardizePathWithoutLocale(string path, ActiveLocale currentLocale)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return PATH_DELIMITER;
            }
            if (!path.StartsWith(PATH_DELIMITER))
            {
                path = PATH_DELIMITER + path;
            }

            // Remove the current locale if it's included in the path
            if (currentLocale != null && path.StartsWith(PATH_DELIMITER + currentLocale.IETFLanguageTag, StringComparison.OrdinalIgnoreCase))
            {
                path = path.Remove(0, currentLocale.IETFLanguageTag.Length + 1);

                // If we accidently removed the starting slash in the above operation, add it again.
                // Example case: path = "en-ca"
                if (!path.StartsWith(PATH_DELIMITER))
                {
                    path = PATH_DELIMITER + path;
                }
            }

            if (path == PATH_DELIMITER)
            {
                return path;
            }

            // Remove trailing slash
            return path.TrimEnd(PATH_DELIMITER[0]);
        }
    }
}
