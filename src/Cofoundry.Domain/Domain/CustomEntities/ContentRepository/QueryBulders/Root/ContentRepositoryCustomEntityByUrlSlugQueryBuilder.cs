﻿using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityByUrlSlugQueryBuilder
    : IContentRepositoryCustomEntityByUrlSlugQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly string _urlSlug;
    private readonly ICustomEntityDefinition _customEntityDefinition;

    public ContentRepositoryCustomEntityByUrlSlugQueryBuilder(
        IExtendableContentRepository contentRepository,
        ICustomEntityDefinition customEntityDefinition,
        string urlSlug
        )
    {
        ArgumentNullException.ThrowIfNull(customEntityDefinition);

        ExtendableContentRepository = contentRepository;
        _customEntityDefinition = customEntityDefinition;
        _urlSlug = urlSlug;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<CustomEntityRenderSummary>> AsRenderSummaries(PublishStatusQuery publishStatusQuery)
    {
        var query = new GetCustomEntityRenderSummariesByUrlSlugQuery()
        {
            CustomEntityDefinitionCode = _customEntityDefinition.CustomEntityDefinitionCode,
            UrlSlug = _urlSlug,
            PublishStatus = publishStatusQuery
        };

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<CustomEntityRenderSummary>> AsRenderSummaries()
    {
        var query = new GetCustomEntityRenderSummariesByUrlSlugQuery()
        {
            CustomEntityDefinitionCode = _customEntityDefinition.CustomEntityDefinitionCode,
            UrlSlug = _urlSlug
        };

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryMutator<IReadOnlyCollection<CustomEntityRenderSummary>, CustomEntityRenderSummary?> AsRenderSummary(PublishStatusQuery publishStatusQuery)
    {
        return AsRenderSummaryInternal(publishStatusQuery);
    }

    public IDomainRepositoryQueryMutator<IReadOnlyCollection<CustomEntityRenderSummary>, CustomEntityRenderSummary?> AsRenderSummary()
    {
        return AsRenderSummaryInternal(null);
    }

    private IDomainRepositoryQueryMutator<IReadOnlyCollection<CustomEntityRenderSummary>, CustomEntityRenderSummary?> AsRenderSummaryInternal(PublishStatusQuery? publishStatusQuery)
    {
        if (!_customEntityDefinition.ForceUrlSlugUniqueness)
        {
            throw new InvalidOperationException($"{nameof(AsRenderSummary)} cannot be used if the custom entity definition has {nameof(_customEntityDefinition.ForceUrlSlugUniqueness)} set to false.");
        }

        var query = new GetCustomEntityRenderSummariesByUrlSlugQuery()
        {
            CustomEntityDefinitionCode = _customEntityDefinition.CustomEntityDefinitionCode,
            UrlSlug = _urlSlug
        };

        if (publishStatusQuery.HasValue)
        {
            query.PublishStatus = publishStatusQuery.Value;
        }

        var mapped = DomainRepositoryQueryContextFactory
            .Create(query, ExtendableContentRepository)
            .Map(r => r.SingleOrDefault());

        return mapped;
    }
}
