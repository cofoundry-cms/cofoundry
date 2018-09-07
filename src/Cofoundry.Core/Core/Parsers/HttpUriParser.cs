using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// A URI parsing helper for http/https based uris.
    /// </summary>
    public static class HttpUriParser
    {
        /// <summary>
        /// Parses a uri string into an absolute uri, ensuring the uri starts with a 
        /// scheme (using http by default). If the scheme is not http or https then
        /// the URI is considered invalid.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>Uri if parsing was successful, otherwise null.</returns>
        public static Uri ParseAbsoluteOrDefault(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            if (!s.Contains("://"))
            {
                s = "http://" + s;
            }
            Uri uri = null;
            if (!Uri.TryCreate(s, UriKind.Absolute, out uri))
            {
                return null;
            }

            if (uri.Scheme == null || !uri.Scheme.ToLower().StartsWith("http"))
            {
                return null;
            }

            return uri;
        }

        /// <summary>
        /// Parses a uri string into a uri. If the uri is absollute then it will ensure 
        /// the uri starts with a scheme (using http by default). If the URI is absolute 
        /// and the scheme is not http or https then the URI is considered invalid.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Uri if parsing was successful, otherwise null</returns>
        public static Uri ParseOrDefault(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            var uri = ParseAbsoluteOrDefault(s);

            if (uri != null) return uri;

            if (!Uri.TryCreate(s, UriKind.Relative, out uri))
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
            if (string.IsNullOrWhiteSpace(s)) return null;

            Uri uri = null;

            if (kind == UriKind.Absolute || kind == UriKind.RelativeOrAbsolute)
            {
                uri = ParseAbsoluteOrDefault(s);
            }

            if (uri != null || kind == UriKind.Absolute) return uri;

            if (!Uri.TryCreate(s, kind, out uri))
            {
                return null;
            }

            return uri;
        }
    }
}
