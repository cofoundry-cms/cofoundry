using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryUserSearchQueryBuilder
    : IContentRepositoryUserSearchQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryUserSearchQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<PagedQueryResult<UserSummary>> AsSummaries(SearchUserSummariesQuery query)
    {
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
