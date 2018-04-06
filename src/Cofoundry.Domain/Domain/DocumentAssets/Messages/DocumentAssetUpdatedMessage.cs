using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a document is updated.
    /// </summary>
    public class DocumentAssetUpdatedMessage
    {
        /// <summary>
        /// Id of the document that has been updated
        /// </summary>
        public int DocumentAssetId { get; set; }

        /// <summary>
        /// Indicates if a new file was uploaded to replace the existing one.
        /// </summary>
        public bool HasFileChanged { get; set; }
    }
}
