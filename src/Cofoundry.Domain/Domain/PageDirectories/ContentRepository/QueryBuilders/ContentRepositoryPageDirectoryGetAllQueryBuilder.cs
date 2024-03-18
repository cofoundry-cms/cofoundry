﻿using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageDirectoryGetAllQueryBuilder
    : IAdvancedContentRepositoryPageDirectoryGetAllQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryPageDirectoryGetAllQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<PageDirectoryRoute>> AsRoutes()
    {
        var query = new GetAllPageDirectoryRoutesQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<PageDirectoryNode> AsTree()
    {
        var query = new GetPageDirectoryTreeQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
