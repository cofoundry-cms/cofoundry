using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Helper for working with relative paths, e.g. "/my-app/myfile.txt".
    /// </summary>
    public class RelativePathHelper
    {
        private const char VIRTUAL_PATH_SEPARATOR = '/';
        private const string VIRTUAL_PATH_SEPARATOR_AS_STRING = "/";

        private static char[] ALL_PATH_SEPARATORS = new char[] { VIRTUAL_PATH_SEPARATOR, '\\' };

        /// <summary>
        /// Combines a series of path parts into a single virtual path
        /// in the format "/path1/path2/filename.txt". 
        /// </summary>
        /// <param name="paths">
        /// The separate path parts to combine into a single
        /// path. These may include leading or trailing path delmiters.
        /// </param>
        /// <returns>The fully combined path, which is always rooted witha forward slash.</returns>
        public static string Combine(params string[] paths)
        {
            var trimmedPaths = paths
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Trim(ALL_PATH_SEPARATORS))
                .Where(p => !string.IsNullOrWhiteSpace(p));

            var result = VIRTUAL_PATH_SEPARATOR + string.Join(VIRTUAL_PATH_SEPARATOR_AS_STRING, trimmedPaths);
            return result;
        }

        /// <summary>
        /// Detects whether two strings are valid full relative paths and the same. 
        /// If the path is a url the comparison ignores the query string and fragment 
        /// portion.
        /// </summary>
        /// <param name="path1">Path to compare.</param>
        /// <param name="path2">Path to compare to.</param>
        public static bool IsWellFormattedAndEqual(string path1, string path2)
        {
            if (StringHelper.IsNullOrWhiteSpace(path1, path2)) return false;

            var cleanPath1 = CleanAndRemoveQueryFromPath(path1);
            var cleanPath2 = CleanAndRemoveQueryFromPath(path2);

            if (!cleanPath1.StartsWith(VIRTUAL_PATH_SEPARATOR_AS_STRING)
                || !cleanPath2.StartsWith(VIRTUAL_PATH_SEPARATOR_AS_STRING)
                || !Uri.IsWellFormedUriString(cleanPath1, UriKind.Relative)
                || !Uri.IsWellFormedUriString(cleanPath2, UriKind.Relative))
            {
                return false;
            }

            return cleanPath1.Equals(cleanPath2, StringComparison.OrdinalIgnoreCase);
        }

        private static string CleanAndRemoveQueryFromPath(string path)
        {
            path = path.TrimStart(new char[] { '~' });

            var queryIndex = path.IndexOf('?');
            if (queryIndex != -1)
            {
                path = path.Remove(queryIndex);
            }

            var fragmentIndex = path.IndexOf('#');
            if (fragmentIndex != -1)
            {
                path = path.Remove(fragmentIndex);
            }
            
            path = path.TrimEnd(VIRTUAL_PATH_SEPARATOR);

            return path;
        }
    }
}
