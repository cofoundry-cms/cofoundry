using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Service for working with mime types.
    /// </summary>
    public class MimeTypeService : IMimeTypeService
    {
        const string DEFAULT_MIME_TYPE = "application/octet-stream";

        private readonly IContentTypeProvider _contentTypeProvider;

        public MimeTypeService(IContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }

        /// <summary>
        /// Finds a mime type that matches the file extension in a file name. Equivalent to 
        /// the old MimeMapping.GetMimeMapping method from .NET 4.x.
        /// </summary>
        /// <param name="fileName">File name with file extension (path optional).</param>
        public string MapFromFileName(string fileName)
        {
            string contentType = null;

            if (_contentTypeProvider.TryGetContentType(fileName, out contentType))
            {
                return contentType;
            }

            return DEFAULT_MIME_TYPE;
        }
    }
}
