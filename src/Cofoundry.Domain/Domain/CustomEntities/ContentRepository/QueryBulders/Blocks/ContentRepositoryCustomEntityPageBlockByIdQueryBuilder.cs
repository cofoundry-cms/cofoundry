﻿using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityPageBlockByIdQueryBuilder
    : IAdvancedContentRepositoryCustomEntityPageBlockByIdQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly int _customEntityBlockId;

    public ContentRepositoryCustomEntityPageBlockByIdQueryBuilder(
        IExtendableContentRepository contentRepository,
        int customEntityBlockId
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityBlockId = customEntityBlockId;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<CustomEntityVersionPageBlockRenderDetails?> AsRenderDetails(PublishStatusQuery? publishStatusQuery = null)
    {
        var query = new GetCustomEntityVersionPageBlockRenderDetailsByIdQuery(_customEntityBlockId, publishStatusQuery);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
