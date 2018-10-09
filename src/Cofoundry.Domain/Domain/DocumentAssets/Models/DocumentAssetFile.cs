using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the file associated with a document asset, including
    /// stream access to the file itself.
    /// </summary>
    public class DocumentAssetFile : IDocumentAssetRenderable
    {
        /// <summary>
        /// Database id of the document asset.
        /// </summary>
        public int DocumentAssetId { get; set; }

        /// <summary>
        /// Mime type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The filename is taken from the title property
        /// and cleaned to remove invalid characters.
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
        /// A stream containing the contents of the file. This needs
        /// to be disposed of when you've finished with it.
        /// </summary>
        public Stream ContentStream { get; set; }

        /// <summary>
        /// Gets the full filename including the file extension. The 
        /// filename is cleaned to remove 
        /// </summary>
        public string GetFileNameWithExtension()
        {
            if (FileName == null) return null;

            var fileName = FilePathHelper.CleanFileName(FileName, DocumentAssetId.ToString());
            var fileNameWithExtension = Path.ChangeExtension(fileName, FileExtension);

            return fileNameWithExtension;
        }
    }
}
