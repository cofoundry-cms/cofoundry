﻿using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageByDirectoryIdQueryBuilder
    : IContentRepositoryPageByDirectoryIdQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly int _pageDirectoryId;

    public ContentRepositoryPageByDirectoryIdQueryBuilder(
        IExtendableContentRepository contentRepository,
        int pageDirectoryId
        )
    {
        ExtendableContentRepository = contentRepository;
        _pageDirectoryId = pageDirectoryId;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<PageRoute>> AsRoutes()
    {
        var query = new GetPageRoutesByPageDirectoryIdQuery(_pageDirectoryId);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
