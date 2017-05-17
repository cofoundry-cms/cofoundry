using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Utilities for working with file paths.
    /// </summary>
    public static class FilePathHelper
    {
        #region constants

        private const char VIRTUAL_PATH_SEPARATOR = '/';
        private const string VIRTUAL_PATH_SEPARATOR_AS_STRING = "/";

        private static char[] ALL_PATH_SEPARATORS = new char[] { VIRTUAL_PATH_SEPARATOR, '\\' };

        #endregion

        /// <summary>
        /// Cleans file names to remove illegal characters
        /// </summary>
        /// <param name="fileName">Filename to clean</param>
        /// <returns>Cleaned file name, or string.Empty if the fileName is null or whitespace</returns>
        public static string CleanFileName(string fileName)
        {
            return CleanFileName(fileName, string.Empty);
        }

        /// <summary>
        /// Cleans file names to remove illegal characters
        /// </summary>
        /// <param name="fileName">Filename to clean</param>
        /// <param name="emptyReplacement">String to use in place of the filename if the result is null or whitespace.</param>
        /// <returns>Cleaned file name, or the emptyReplacement value if the fileName is null or whitespace</returns>
        public static string CleanFileName(string fileName, string emptyReplacement)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return emptyReplacement;

            var invalidChars = Path.GetInvalidFileNameChars();

            var chars = fileName
                .Where(x => !invalidChars.Contains(x))
                .ToArray();

            var newName = new string(chars);

            if (string.IsNullOrWhiteSpace(newName)) return emptyReplacement;
            return newName;
        }

        public static string CombineVirtualPath(params string[] paths)
        {
            var trimmedPaths = paths
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Trim(VIRTUAL_PATH_SEPARATOR));

            var result = VIRTUAL_PATH_SEPARATOR + string.Join(VIRTUAL_PATH_SEPARATOR_AS_STRING, trimmedPaths);
            return result;
        }
    }
}
