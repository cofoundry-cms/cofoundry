using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryRoleByIdRangeQueryBuilder
    : IContentRepositoryRoleByIdRangeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly IReadOnlyCollection<int> _roleIds;

    public ContentRepositoryRoleByIdRangeQueryBuilder(
        IExtendableContentRepository contentRepository,
        IReadOnlyCollection<int> roleIds
        )
    {
        ExtendableContentRepository = contentRepository;
        _roleIds = roleIds;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<int, RoleMicroSummary>> AsMicroSummaries()
    {
        var query = new GetRoleMicroSummariesByIdRangeQuery(_roleIds);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
