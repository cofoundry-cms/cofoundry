using Conditions;
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
            Condition.Requires(fileName).IsNotNullOrWhiteSpace();
            Condition.Requires(mimeType).IsNotNullOrWhiteSpace();
            Condition.Requires(fileLength).IsGreaterThan(0);
            Condition.Requires(stream).IsNotNull();

            FileName = fileName;
            MimeType = mimeType;
            FileLength = FileLength;
            _stream = stream;
        }

        public string FileName { get; private set; }

        public string MimeType { get; private set; }

        public long FileLength { get; private set; }

        public Task<System.IO.Stream> GetFileStreamAsync()
        {
            return Task.FromResult(_stream);
        }
    }
}
