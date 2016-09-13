using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    public static class UriParse
    {
        /// <summary>
        /// Parses a uri string into an absolute uri, ensuring the uri starts with a scheme (using http by default)
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Uri if parsing was successful, otherwise null</returns>
        public static Uri ParseAbsoluteOrDefault(string s)
        {
            if (!s.Contains("://"))
            {
                s = "http://" + s;
            }
            Uri uri = null;
            if (!Uri.TryCreate(s, UriKind.Absolute, out uri))
            {
                return null;
            }
            return uri;
        }

        /// <summary>
        /// Parses a uri string into an uri using UriKind.RelativeOrAbsolute, returning
        /// null if the string does not parse.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Uri if parsing was successful, otherwise null</returns>
        public static Uri ParseOrDefault(string s)
        {
            Uri uri = null;
            if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out uri))
            {
                return null;
            }
            return uri;
        }

        /// <summary>
        /// Parses a uri string into an uri using UriKind.RelativeOrAbsolute, returning
        /// null if the string does not parse.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="kind">UriKind to parse the string as</param>
        /// <returns>Uri if parsing was successful, otherwise null</returns>
        public static Uri ParseOrDefault(string s, UriKind kind)
        {
            Uri uri = null;
            if (!Uri.TryCreate(s, kind, out uri))
            {
                return null;
            }
            return uri;
        }
    }
}
