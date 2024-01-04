using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryGetAllPermissionsQueryBuilder
    : IAdvancedContentRepositoryGetAllPermissionsQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryGetAllPermissionsQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<IPermission>> AsIPermission()
    {
        var query = new GetAllPermissionsQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
