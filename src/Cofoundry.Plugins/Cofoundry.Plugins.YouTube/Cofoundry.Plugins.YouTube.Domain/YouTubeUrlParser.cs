using System.Text.RegularExpressions;

namespace Cofoundry.Plugins.YouTube.Domain;

/// <summary>
/// Parser for working with youtube urls and ids
/// </summary>
public static partial class YouTubeUrlParser
{
    /// <summary>
    /// Extracts the ID of a video from a YouTube URL.
    /// </summary>
    /// <see href="http://stackoverflow.com/questions/3652046/c-sharp-regex-to-get-video-id-from-youtube-and-vimeo-by-url"/>
    /// <param name="youTubeVideoUrl">You tube video URL.</param>
    /// <returns>The id of the YouTube video</returns>
    public static string GetId(string youTubeVideoUrl)
    {
        var match = YouTubeRegex().Match(youTubeVideoUrl);

        if (!match.Success)
        {
            return string.Empty;
        }

        return match.Groups[1].Value;
    }

    [GeneratedRegex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-GB")]
    private static partial Regex YouTubeRegex();
}
