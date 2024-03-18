﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

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
    [return: NotNullIfNotNull(nameof(dbDocument))]
    DocumentAssetSummary? Map(DocumentAsset? dbDocument);
}
