using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Library of useful regular expressions.
    /// </summary>
    public static class RegexLibary
    {
        /// <summary>
        /// Matches a valid url slug of lowercase characters, numbers, hyphens and underscores.
        /// </summary>
        public const string Slug = "^[a-z0-9-_]+$";

        /// <summary>
        /// Matches a valid url slug of characters (upper or lower), hyphens and underscores.
        /// </summary>
        public const string SlugCaseInsensitive = "^[a-zA-Z0-9-_]+$";

        /// <summary>
        /// Matches upper case alpha-numerical values. E.g. ER1, ABC or 123
        /// </summary>
        public const string AlphaNumericalUpper = "^[A-Z0-9]+$";

        /// <summary>
        /// Extracts a youtube video id out of a url. The id can be found in the 8th match.
        /// </summary>
        public const string YouTubeVideoLink = @"^.*((youtu.be\/)|(v\/)|(\/u\/\w\/)|(embed\/)|(watch\?))\??v?=?([^#\&\?]*).*";

        /// <summary>
        /// Extracts a vimeo video id out of a url. The id can be found in the 4th match.
        /// </summary>
        public const string VimeoVideoLink = @"(https?:\/\/)?(?:www\.)?vimeo.com\/(?:channels\/|groups\/([^\/]*)\/videos\/|album\/(\d+)\/video\/|)(\d+)(?:$|\/|\?)";

    }
}
