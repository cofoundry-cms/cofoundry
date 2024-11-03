namespace Cofoundry.Plugins.YouTube.Domain;

/// <summary>
/// YouTube video information extracted from oembed data.
/// </summary>
public class YouTubeVideo
{
    /// <summary>
    /// The unique 11 charcter video id.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Title of the video.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description of the video.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The date the video was published on YouTube.
    /// </summary>
    public DateTimeOffset? PublishDate { get; set; }

    /// <summary>
    /// The full url to the thumbnail image representing the video.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// The width of the thumnbail image.
    /// </summary>
    public int? ThumbnailWidth { get; set; }

    /// <summary>
    /// The height of the thumnbail image.
    /// </summary>
    public int? ThumbnailHeight { get; set; }
}
