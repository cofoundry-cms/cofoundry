using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cofoundry.Domain
{
    public class DocumentAssetFile
    {
        public int DocumentAssetId { get; set; }

        /// <summary>
        /// Mime type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Name of the file including extension
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// A stream containing the contents of the file. This needs
        /// to be disposed of when you've finished with it.
        /// </summary>
        public Stream ContentStream { get; set; }
    }
}
