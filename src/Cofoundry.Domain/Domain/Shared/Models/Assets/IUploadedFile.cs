using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IUploadedFile
    {
        string FileName { get; }
        string MimeType { get; }
        long FileLength { get; }
        Task<Stream> GetFileStreamAsync();
    }
}
