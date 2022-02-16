using Cofoundry.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Cofoundry <see cref="IFileSource"/> abstraction over an ASP.NET 
    /// <see cref="IFormFile"/>.
    /// </summary>
    /// <inheritdoc/>
    public class FormFileSource : IFileSource
    {
        private IFormFile _formFile = null;

        public FormFileSource(IFormFile formFile)
        {
            if (formFile == null) throw new ArgumentNullException(nameof(formFile));

            _formFile = formFile;

            FileName = _formFile.FileName;
            MimeType = _formFile.ContentType;
            FileLength = _formFile.Length;
        }

        /// <summary>
        /// The name of the file including file extension (if available). The
        /// file name is specified by the client and so cannot be trusted.
        /// </summary>
        [Required]
        public string FileName { get; private set; }

        /// <summary>
        /// The mime/content type associated with the file. This is specified 
        /// by the client and is not to be trusted.
        /// </summary>
        public string MimeType { get; private set; }

        /// <summary>
        /// The total length of the file in bytes.
        /// </summary>
        public long FileLength { get; private set; }

        public Task<Stream> OpenReadStreamAsync()
        {
            return Task.FromResult(_formFile.OpenReadStream());
        }
    }
}