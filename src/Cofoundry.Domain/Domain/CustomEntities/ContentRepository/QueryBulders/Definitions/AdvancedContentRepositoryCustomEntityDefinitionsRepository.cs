using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class AdvancedContentRepositoryCustomEntityDefinitionsRepository
        : IAdvancedContentRepositoryCustomEntityDefinitionsRepository
        , IExtendableContentRepositoryPart
{
    public AdvancedContentRepositoryCustomEntityDefinitionsRepository(
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

    public IContentRepositoryCustomEntityDefinitionByCodeQueryBuilder GetByCode(string customEntityDefinitionCode)
    {
        return new ContentRepositoryCustomEntityDefinitionByCodeQueryBuilder(ExtendableContentRepository, customEntityDefinitionCode);
    }

    public IAdvancedContentRepositoryCustomEntityDefinitionByDisplayModelTypeQueryBuilder GetByDisplayModelType(Type displayModelType)
    {
        return new AdvancedContentRepositoryCustomEntityDefinitionByDisplayModelTypeQueryBuilder(ExtendableContentRepository, displayModelType);
    }

    public Task EnsureExistsAsync(string customEntityDefinitionCode)
    {
        var command = new EnsureCustomEntityDefinitionExistsCommand(customEntityDefinitionCode);
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }
}