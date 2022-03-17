using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityDataModelSchemasRepository
        : IAdvancedContentRepositoryCustomEntityDataModelSchemasRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryCustomEntityDataModelSchemasRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder GetByCustomEntityDefinitionCode(string customEntityDefinitionCode)
    {
        return new ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder(ExtendableContentRepository, customEntityDefinitionCode);
    }

    public IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder GetByCustomEntityDefinitionCodeRange(IEnumerable<string> customEntityDefinitionCodes)
    {
        return new ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder(ExtendableContentRepository, customEntityDefinitionCodes);
    }
}