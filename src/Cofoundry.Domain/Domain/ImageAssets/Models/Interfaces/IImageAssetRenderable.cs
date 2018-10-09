using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Can be used to mark a model as having all the information required to
    /// render out a url to the image file.
    /// </summary>
    public interface IImageAssetRenderable
    {
        /// <summary>
        /// Database if of the image asset.
        /// </summary>
        int ImageAssetId { get; set; }

        /// <summary>
        /// Original filename without an extension.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// File name used internally for storing the file on disk (without 
        /// extension). This is typically in the format {ImageAssetId}-{FileStamp}.
        /// </summary>
        /// <remarks>
        /// For files created before file stamps were used this may
        /// contain only the image asset id.
        /// </remarks>
        string FileNameOnDisk { get; set; }

        /// <summary>
        /// Original file extension without the leading dot.
        /// </summary>
        string FileExtension { get; set; }

        /// <summary>
        /// The width of the image file in pixels.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// The height of the image file in pixels.
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// The title or alt text for an image. Recommended to be up 125 characters 
        /// to accomodate screen readers.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// An identifier linked to the physical file that can be used for
        /// cache busting. By default this is a timestamp.
        /// </summary>
        string FileStamp { get; set; }

        /// <summary>
        /// A random string token that can be used to verify a file request
        /// and mitigate enumeration attacks.
        /// </summary>
        string VerificationToken { get; set; }

        /// <summary>
        /// The default Anchor Location when using dynamic cropping
        /// </summary>
        ImageAnchorLocation? DefaultAnchorLocation { get; set; }
    }
}
