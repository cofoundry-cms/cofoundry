using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder
    : IAdvancedContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<PagedQueryResult<CustomEntityVersionSummary>> AsVersionSummaries(GetCustomEntityVersionSummariesByCustomEntityIdQuery query)
    {
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
