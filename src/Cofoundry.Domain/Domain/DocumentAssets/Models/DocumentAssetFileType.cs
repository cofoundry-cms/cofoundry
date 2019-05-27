using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a file type currently stored in the document assets
    /// database table.
    /// </summary>
    public class DocumentAssetFileType
    {
        /// <summary>
        /// The file system file extension without the
        /// leading dot.
        /// </summary>
        public string FileExtension { get; set; }
    }
}
