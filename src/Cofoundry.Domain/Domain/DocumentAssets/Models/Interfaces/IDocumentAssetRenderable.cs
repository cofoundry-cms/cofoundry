using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Can be used to mark a model as having all the information required to
    /// render out a url to the document file.
    /// </summary>
    public interface IDocumentAssetRenderable
    {
        /// <summary>
        /// Database id of the document asset.
        /// </summary>
        int DocumentAssetId { get; set; }

        /// <summary>
        /// The filename is taken from the title property
        /// and cleaned to remove invalid characters.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Original file extension without the leading dot.
        /// </summary>
        string FileExtension { get; set; }

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
    }
}
