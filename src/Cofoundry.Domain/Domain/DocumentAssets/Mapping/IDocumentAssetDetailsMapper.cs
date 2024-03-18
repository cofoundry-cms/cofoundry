﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to DocumentAssetDetails objects.
/// </summary>
public interface IDocumentAssetDetailsMapper
{
    /// <summary>
    /// Maps an EF DocumentAsset record from the db into a DocumentAssetDetails 
    /// object. If the db record is null then null is returned.
    /// </summary>
    /// <param name="dbDocument">DocumentAsset record from the database.</param>
    [return: NotNullIfNotNull(nameof(dbDocument))]
    DocumentAssetDetails? Map(DocumentAsset? dbDocument);
}
