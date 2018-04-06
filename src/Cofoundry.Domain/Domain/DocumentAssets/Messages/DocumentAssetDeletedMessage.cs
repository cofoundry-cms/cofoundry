using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a document is deleted.
    /// </summary>
    public class DocumentAssetDeletedMessage
    {
        /// <summary>
        /// Id of the document asset that has been deleted.
        /// </summary>
        public int DocumentAssetId { get; set; }
    }
}
