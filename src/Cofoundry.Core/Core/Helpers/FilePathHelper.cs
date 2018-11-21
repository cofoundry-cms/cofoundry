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

        private static char[] FILE_NAME_INVALID_CHARS = new char[] { '\0', '*', '<', '>', '/', '\\', ':', '?', '|', '\"' };
        private static char[] FILE_NAME_TRIM_CHARS = new char[] { '$', ' ' };
        private static string[] FILE_NAME_INVALID_VALUES = new string[]
        {
                "CON", "PRN", "AUX", "NUL",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        #endregion

        /// <summary>
        /// Cleans file names to remove illegal characters. The difficulties
        /// around valid filenames across different file systems means
        /// that this isn't perfect, but does cover all known problem characters
        /// and reserved names for common file systems.
        /// </summary>
        /// <param name="fileName">Filename to clean.</param>
        /// <returns>Cleaned file name, or string.Empty if the fileName is null or whitespace.</returns>
        public static string CleanFileName(string fileName)
        {
            return CleanFileName(fileName, string.Empty);
        }

        /// <summary>
        /// Cleans file names to remove illegal characters. The difficulties
        /// around valid filenames across different file systems means
        /// that this isn't perfect, but does cover all known problem characters
        /// and reserved names for common file systems.
        /// </summary>
        /// <param name="fileName">Filename to clean.</param>
        /// <param name="emptyReplacement">String to use in place of the filename if the result is null or whitespace.</param>
        /// <returns>Cleaned file name, or the emptyReplacement value if the fileName is null or whitespace.</returns>
        public static string CleanFileName(string fileName, string emptyReplacement)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return emptyReplacement;
            
            var validChars = fileName.Where(x => 
                !Char.IsControl(x) 
                && !FILE_NAME_INVALID_CHARS.Contains(x)
                );

            var newName = String.Concat(validChars).Trim(FILE_NAME_TRIM_CHARS);

            if (string.IsNullOrWhiteSpace(newName) || FILE_NAME_INVALID_VALUES.Contains(newName))
            {
                return emptyReplacement;
            }

            return newName;
        }

        /// <summary>
        /// <para>
        /// Determines if the specified file extension contains
        /// invalid characters that would prevent it being written
        /// to disk. 
        /// </para>
        /// <para>
        /// The difficulties around valid filenames across different file systems means
        /// that this isn't perfect, but does cover all known problem characters
        /// for common file systems.
        /// </para>
        /// </summary>
        /// <param name="fileExtension">The file extension, the leading dot is optional e.g. ".gif", "gif".</param>
        public static bool FileExtensionContainsInvalidChars(string fileExtension)
        {
            var trimmedExtension = fileExtension?.TrimStart('.');

            if (string.IsNullOrWhiteSpace(trimmedExtension)) return false;

            var hasInvalidChars = trimmedExtension.Any(x =>
                x == '.'
                || Char.IsControl(x)
                || FILE_NAME_INVALID_CHARS.Contains(x)
                );

            return hasInvalidChars;
        }
    }
}
