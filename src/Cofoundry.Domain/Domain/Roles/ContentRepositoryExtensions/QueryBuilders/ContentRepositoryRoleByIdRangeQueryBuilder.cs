using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryRoleByIdRangeQueryBuilder
    : IContentRepositoryRoleByIdRangeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly IEnumerable<int> _roleIds;

    public ContentRepositoryRoleByIdRangeQueryBuilder(
        IExtendableContentRepository contentRepository,
        IEnumerable<int> roleIds
        )
    {
        ExtendableContentRepository = contentRepository;
        _roleIds = roleIds;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IDictionary<int, RoleMicroSummary>> AsMicroSummaries()
    {
        var query = new GetRoleMicroSummariesByIdRangeQuery(_roleIds);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
