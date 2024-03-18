﻿using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityByDefinitionQueryBuilder
    : IContentRepositoryCustomEntityByDefinitionQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly string _customEntityDefinitionCode;

    public ContentRepositoryCustomEntityByDefinitionQueryBuilder(
        IExtendableContentRepository contentRepository,
        string customEntityDefinitionCode
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityDefinitionCode = customEntityDefinitionCode;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<CustomEntityRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatusQuery = null)
    {
        var query = new GetCustomEntityRenderSummariesByDefinitionCodeQuery(_customEntityDefinitionCode);

        if (publishStatusQuery.HasValue)
        {
            query.PublishStatus = publishStatusQuery.Value;
        }

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<CustomEntityRoute>> AsRoutes()
    {
        var query = new GetCustomEntityRoutesByDefinitionCodeQuery(_customEntityDefinitionCode);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
