﻿using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryImageAssetByIdRangeQueryBuilder
    : IContentRepositoryImageAssetByIdRangeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly IEnumerable<int> _imageAssetIds;

    public ContentRepositoryImageAssetByIdRangeQueryBuilder(
        IExtendableContentRepository contentRepository,
        IEnumerable<int> imageAssetIds
        )
    {
        ExtendableContentRepository = contentRepository;
        _imageAssetIds = imageAssetIds;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<int, ImageAssetRenderDetails>> AsRenderDetails()
    {
        var query = new GetImageAssetRenderDetailsByIdRangeQuery(_imageAssetIds);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
