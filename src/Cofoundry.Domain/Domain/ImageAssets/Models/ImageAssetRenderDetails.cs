using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains all the basic information required to render out an image asset
    /// to a page, including all the data needed to construct an asset file 
    /// url.
    /// </summary>
    public class ImageAssetRenderDetails : IImageAssetRenderable
    {
        /// <summary>
        /// Database if of the image asset.
        /// </summary>
        public int ImageAssetId { get; set; }

        /// <summary>
        /// Original filename without an extension.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File name used internally for storing the file on disk (without 
        /// extension). This is typically in the format {ImageAssetId}-{FileStamp}.
        /// </summary>
        /// <remarks>
        /// For files created before file stamps were used this may
        /// contain only the image asset id.
        /// </remarks>
        public string FileNameOnDisk { get; set; }

        /// <summary>
        /// Original file extension without the leading dot.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// The width of the image file in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the image file in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The title or alt text for an image. Recommended to be up 125 characters 
        /// to accomodate screen readers.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// An identifier linked to the physical file that can be used for
        /// cache busting. By default this is a timestamp.
        /// </summary>
        public string FileStamp { get; set; }

        /// <summary>
        /// A random string token that can be used to verify a file request
        /// and mitigate enumeration attacks.
        /// </summary>
        public string VerificationToken { get; set; }

        /// <summary>
        /// The date the file was last updated. Used for cache busting
        /// the asset file in web requests.
        /// </summary>
        public DateTime FileUpdateDate { get; set; }

        /// <summary>
        /// The default Anchor Location when using dynamic cropping
        /// </summary>
        public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

        /// <summary>
        /// The base url to display the image.
        /// </summary>
        public string Url { get; set; }
    }
}
