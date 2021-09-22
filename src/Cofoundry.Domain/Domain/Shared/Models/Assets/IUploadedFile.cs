using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Abstraction of a file in the process of being uploaded. In ASP.Net terms
    /// this is an abstraction ofver IFormFile but has broader scope and can be used 
    /// to represent files uploaded to Cofoundry by other mechanisms.
    /// </summary>
    public interface IUploadedFile
    {
        /// <summary>
        /// The name of the file including file extension (if available). For some
        /// implementations the file name is specified by the client and so cannot 
        /// be trusted.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Optional mime/content type associated with the file, if it is known. For some 
        /// implementations the mime type is specified by the client and is not to be 
        /// trusted.
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// Optional total length of the file in bytes, if it is known.
        /// </summary>
        long FileLength { get; }

        /// <summary>
        /// Opens a stream of the file contents. The callee is responsible for disposing 
        /// of the stream.
        /// </summary>
        Task<Stream> OpenReadStreamAsync();
    }
}
