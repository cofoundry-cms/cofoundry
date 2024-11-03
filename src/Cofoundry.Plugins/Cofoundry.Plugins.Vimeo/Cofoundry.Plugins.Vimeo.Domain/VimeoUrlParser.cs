using System.Text.RegularExpressions;

namespace Cofoundry.Plugins.Vimeo.Domain;

/// <summary>
/// Parser for working with vimeo urls and ids
/// </summary>
public static partial class VimeoUrlParser
{
    /// <summary>
    /// Extracts the ID of a video from a Vimeo URL.
    /// </summary>
    /// <param name="vimeoVideoUrl">The vimeo video URL.</param>
    /// <returns>The id of the Vimeo video, or empty string if the id couldn't be extracted</returns>
    public static string GetId(this string vimeoVideoUrl)
    {
        var match = VimeoRegex().Match(vimeoVideoUrl);

        if (!match.Success)
        {
            return string.Empty;
        }

        return match.Groups[5].Value;
    }

    [GeneratedRegex(@"^.*(vimeo\.com\/)((channels\/[A-z]+\/)|(groups\/[A-z]+\/videos\/))?([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-GB")]
    private static partial Regex VimeoRegex();
}
