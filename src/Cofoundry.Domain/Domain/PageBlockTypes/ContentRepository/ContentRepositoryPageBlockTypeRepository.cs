using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageBlockTypeRepository
        : IAdvancedContentRepositoryPageBlockTypeRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryPageBlockTypeRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IContentRepositoryPageBlockTypeByIdQueryBuilder GetById(int pageBlockTypeId)
    {
        return new ContentRepositoryPageBlockTypeByIdQueryBuilder(ExtendableContentRepository, pageBlockTypeId);
    }

    public IContentRepositoryPageBlockTypeGetAllQueryBuilder GetAll()
    {
        return new ContentRepositoryPageBlockTypeGetAllQueryBuilder(ExtendableContentRepository);
    }
}