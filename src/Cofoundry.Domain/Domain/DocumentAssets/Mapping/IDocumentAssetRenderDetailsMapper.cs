using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to DocumentAssetRenderDetails objects.
    /// </summary>
    public interface IDocumentAssetRenderDetailsMapper
    {
        /// <summary>
        /// Maps an EF DocumentAsset record from the db into a DocumentAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbDocument">DocumentAsset record from the database.</param>
        DocumentAssetRenderDetails Map(DocumentAsset dbDocument);
    }
}
