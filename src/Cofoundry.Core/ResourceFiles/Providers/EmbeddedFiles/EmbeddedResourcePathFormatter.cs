using System;
using System.IO;
using System.Linq;

namespace Cofoundry.Core.ResourceFiles
{
    /// <summary>
    /// Utilities for working with embedded resource paths.
    /// </summary>
    public static class EmbeddedResourcePathFormatter
    {
        /// <remarks>
        /// This list of characters to be replaced is taken from 
        /// https://docs.microsoft.com/en-us/dotnet/api/system.resources.tools.stronglytypedresourcebuilder.verifyresourcename?redirectedfrom=MSDN&view=netframework-4.8#remarks
        /// Further exclusions apply, including edge cases with reserved 
        /// words but until proven otherwise we'll assume that don't need 
        /// to be covered.
        /// </remarks>
        private static char[] INVALID_CHARS = new char[] { ' ', '\u00A0', ',', ';', '~', '@', '#', '%', '^', '&', '*', '+', '-', '/', '\\', '<', '>', '?', '[', ']', '(', ')', '{', '}', '"', '\'', ':', '!' };
        private static char[] PATH_DELIMITERS = new char[] { '/', '\\' };
        const string VIRTUAL_PATH_PREFIX = "~/";
        const char EMBEDDED_DIRECTORY_DELIMITER = '.';
        const char INVALID_CHARACTER_REPLACEMENT = '_';

        /// <summary>
        /// Convert a virtual directory (without filename/extension) to an
        /// embedded resource path, trimming any leading or trailing path
        /// delimiters.
        /// </summary>
        /// <param name="path">
        /// A virtual path to convert. The path cannot contain a filename 
        /// and file extension, as the period in the file extension conflicts
        /// with the embedded resource delimiter and would be replaced erroneously.
        /// </param>
        /// <returns>
        /// A formatted embedded resource path without leading or trailing path 
        /// delimiters. If path is null, then null is returned.
        /// </returns>
        public static string ConvertFromVirtualDirectory(string path)
        {
            if (path == null) return null;
            var result = path;

            if (path.StartsWith(VIRTUAL_PATH_PREFIX))
            {
                result = path.Remove(0, VIRTUAL_PATH_PREFIX.Length);
            }

            var filteredCharacters = result
                // in a file path '.' need to be replaced, a file name/extension should not be supplied to this method
                .Select(c => c == EMBEDDED_DIRECTORY_DELIMITER ? INVALID_CHARACTER_REPLACEMENT : c)
                // replace file path delimiters with the embedded resource equivalent '.'
                .Select(c => PATH_DELIMITERS.Contains(c) ? EMBEDDED_DIRECTORY_DELIMITER : c)
                // replace any other invalid characters
                .Select(c => INVALID_CHARS.Contains(c) ? INVALID_CHARACTER_REPLACEMENT : c)
                ;

            result = string
                .Concat(filteredCharacters)
                .Trim(EMBEDDED_DIRECTORY_DELIMITER);

            return result;
        }

        /// <summary>
        /// <para>
        /// Formats an embedded resource path to be a valid embedded resource 
        /// path, replacing any invalid characters with underscores. 
        /// Leading and trailing periods are trimmed from the result.
        /// </para>
        /// </summary>
        /// <param name="path">
        /// The path to format. This should be an embedded resource path  
        /// e.g. the invalid space in "My_Directory.My SubDirectory" will be 
        /// fixed to be "My_Directory.My_SubDirectory".
        /// </param>
        /// <returns>
        /// The resulting valid embedded resource path. If path is null, 
        /// then null is returned.
        /// </returns>
        public static string CleanPath(string path)
        {
            if (path == null) return null;

            var result = path;

            var filteredCharacters = result
                .Select(c => INVALID_CHARS.Contains(c) ? INVALID_CHARACTER_REPLACEMENT : c)
                ;

            result = string
                .Concat(filteredCharacters)
                .Trim(EMBEDDED_DIRECTORY_DELIMITER);

            return result;
        }
    }
}
