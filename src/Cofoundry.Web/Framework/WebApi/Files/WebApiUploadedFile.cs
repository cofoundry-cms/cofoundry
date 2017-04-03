using Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cofoundry.Domain;
using Cofoundry.Core.Web;

namespace Cofoundry.Web.WebApi
{
    public class WebApiUploadedFile : IUploadedFile
    {
        private HttpContent _fileData = null;

        public WebApiUploadedFile(
            HttpContent fileData, 
            IMimeTypeService mimeTypeService
            )
        {
            Condition.Requires(fileData, "fileData").IsNotNull();

            _fileData = fileData;
            FileName = fileData.Headers.ContentDisposition.FileName.Trim('\"');
            MimeType = string.IsNullOrEmpty(fileData.Headers.ContentType.MediaType) ? mimeTypeService.MapFromFileName(FileName) : fileData.Headers.ContentType.MediaType;
            FileLength = fileData.Headers.ContentLength.Value;
        }

        [Required]
        [NotDangerousFileExtension]
        public string FileName { get; private set; }

        [Required]
        [NotDangerousMimeType]
        public string MimeType { get; private set; }

        [Required]
        public long FileLength { get; private set; }

        public async Task<Stream> GetFileStreamAsync()
        {
            return await _fileData.ReadAsStreamAsync();
        }
    }
}
