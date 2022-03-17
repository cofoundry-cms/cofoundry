using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityDefinitionsRepository
        : IContentRepositoryCustomEntityDefinitionsRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryCustomEntityDefinitionsRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder GetAll()
    {
        return new ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder(ExtendableContentRepository);
    }

    public IContentRepositoryCustomEntityDefinitionByCodeQueryBuilder GetByCode(string customEntityDefinitioncode)
    {
        return new ContentRepositoryCustomEntityDefinitionByCodeQueryBuilder(ExtendableContentRepository, customEntityDefinitioncode);
    }
}
