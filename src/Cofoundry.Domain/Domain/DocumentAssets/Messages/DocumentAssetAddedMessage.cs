using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a document is added.
    /// </summary>
    public class DocumentAssetAddedMessage
    {
        /// <summary>
        /// Id of the document that was added.
        /// </summary>
        public int DocumentAssetId { get; set; }
    }
}
