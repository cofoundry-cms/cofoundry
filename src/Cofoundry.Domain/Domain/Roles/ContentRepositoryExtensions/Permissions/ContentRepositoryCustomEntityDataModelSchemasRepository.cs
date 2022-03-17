using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPermissionsRepository
        : IAdvancedContentRepositoryPermissionsRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryPermissionsRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IAdvancedContentRepositoryGetAllPermissionsQueryBuilder GetAll()
    {
        return new ContentRepositoryGetAllPermissionsQueryBuilder(ExtendableContentRepository);
    }
}