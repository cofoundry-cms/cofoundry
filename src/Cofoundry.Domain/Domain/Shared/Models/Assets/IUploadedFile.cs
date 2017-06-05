using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Abstraction of a file in the process of being uploaded. In asp.net terms
    /// this is an abstraction ofver IFormFile but has broader scope and can be used 
    /// to represent files uploaded to Cofoundry by other mechanisms.
    /// </summary>
    public interface IUploadedFile
    {
        /// <summary>
        /// The name of the file including file extension (if available). The
        /// file name is specified by the client and so cannot be trusted.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// The mime/content type associated with the file. This is specified 
        /// by the client and is not to be trusted.
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// The total length of the file in bytes.
        /// </summary>
        long FileLength { get; }

        /// <summary>
        /// Returns a reference to the underlaying stream 
        /// for the file.
        /// </summary>
        Task<Stream> OpenReadStreamAsync();
    }
}
