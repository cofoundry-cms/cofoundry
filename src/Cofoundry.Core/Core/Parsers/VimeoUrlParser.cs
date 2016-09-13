using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Parser for working with vimeo urls and ids
    /// </summary>
    public static class VimeoUrlParser
    {
        /// <summary>
        /// Extracts the ID of a video from a Vimeo URL.
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/3652046/c-sharp-regex-to-get-video-id-from-youtube-and-vimeo-by-url"/>
        /// <param name="vimeoVideoUrl">The vimeo video URL.</param>
        /// <returns>The id of the Vimeo video, or empty string if the id couldn't be extracted</returns>
        public static string GetId(this string vimeoVideoUrl)
        {
            Regex regex = new Regex(@"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            Match match = regex.Match(vimeoVideoUrl);

            if (!match.Success)
                return "";

            return match.Groups[1].Value;
        }
    }
}
