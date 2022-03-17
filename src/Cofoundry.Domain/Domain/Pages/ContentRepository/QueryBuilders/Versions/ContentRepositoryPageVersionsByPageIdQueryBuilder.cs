using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageVersionsByPageIdQueryBuilder
    : IAdvancedContentRepositoryPageVersionsByPageIdQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryPageVersionsByPageIdQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<PagedQueryResult<PageVersionSummary>> AsVersionSummaries(GetPageVersionSummariesByPageIdQuery query)
    {
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
