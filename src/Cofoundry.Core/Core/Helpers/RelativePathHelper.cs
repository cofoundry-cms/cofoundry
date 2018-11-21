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
    }
}
