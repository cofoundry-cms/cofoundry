using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Http;

namespace Cofoundry.Web
{
    /// <summary>
    /// Cofoundry IUploadedFile abstraction over an asp.net IFormFile.
    /// </summary>
    public class FormFileUploadedFile : IUploadedFile
    {
        private IFormFile _formFile = null;

        public FormFileUploadedFile(
            IFormFile formFile, 
            IMimeTypeService mimeTypeService
            )
        {
            if (formFile == null) throw new ArgumentNullException(nameof(formFile));

            _formFile = formFile;

            FileName = _formFile.FileName;
            MimeType = mimeTypeService.MapFromFileName(_formFile.FileName, _formFile.ContentType);
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
        [Required]
        public string MimeType { get; private set; }

        /// <summary>
        /// The total length of the file in bytes.
        /// </summary>
        [Required]
        public long FileLength { get; private set; }

        /// <summary>
        /// Returns a reference to the underlaying stream 
        /// for the file.
        /// </summary>
        public Task<Stream> OpenReadStreamAsync()
        {
            return Task.FromResult(_formFile.OpenReadStream());
        }
    }
}
