using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A basic IUploadedFile that gives you complete control over the file stream.
    /// </summary>
    public class StreamFileSource : IUploadedFile
    {
        private readonly Stream _stream;

        public StreamFileSource(string fileName, string mimeType, long fileLength, Stream stream)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentEmptyException(nameof(fileName));

            if (mimeType == null) throw new ArgumentNullException(nameof(mimeType));
            if (string.IsNullOrWhiteSpace(mimeType)) throw new ArgumentEmptyException(nameof(mimeType));

            if (fileLength < 1) throw new ArgumentOutOfRangeException(nameof(fileLength));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            FileName = fileName;
            MimeType = mimeType;
            FileLength = FileLength;
            _stream = stream;
        }

        public string FileName { get; private set; }

        public string MimeType { get; private set; }

        public long FileLength { get; private set; }

        public Task<System.IO.Stream> OpenReadStreamAsync()
        {
            return Task.FromResult(_stream);
        }
    }
}
