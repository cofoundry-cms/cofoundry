using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to DocumentAssetDetails objects.
    /// </summary>
    public interface IDocumentAssetSummaryMapper
    {
        /// <summary>
        /// Maps an EF DocumentAsset record from the db into a DocumentAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbDocument">DocumentAsset record from the database.</param>
        DocumentAssetSummary Map(DocumentAsset dbDocument);
    }
}
