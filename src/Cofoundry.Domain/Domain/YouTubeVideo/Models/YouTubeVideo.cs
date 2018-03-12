using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// YouTube video information extracted from oembed data.
    /// </summary>
    public class YouTubeVideo
    {
        /// <summary>
        /// The unique 11 charcter video id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Title of the video.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The default height of the embedded video player.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// The date the video was uploaded to YouTube.
        /// </summary>
        public DateTime? UploadDate { get; set; }

        /// <summary>
        /// The default width of the embedded video player.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// The full url to the default thumbnail image representing the video.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// The width of the thumnbail image.
        /// </summary>
        public int? ThumbnailWidth { get; set; }

        /// <summary>
        /// The height of the thumnbail image.
        /// </summary>
        public int? ThumbnailHeight { get; set; }
    }
}