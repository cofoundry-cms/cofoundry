using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryRoleSearchQueryBuilder
    : IContentRepositoryRoleSearchQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryRoleSearchQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<PagedQueryResult<RoleMicroSummary>> AsMicroSummaries(SearchRolesQuery query)
    {
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
