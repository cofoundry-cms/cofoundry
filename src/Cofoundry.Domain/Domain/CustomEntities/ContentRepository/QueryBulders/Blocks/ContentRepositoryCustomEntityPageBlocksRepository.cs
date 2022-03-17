using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityPageBlocksRepository
        : IAdvancedContentRepositoryCustomEntityPageBlocksRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryCustomEntityPageBlocksRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IAdvancedContentRepositoryCustomEntityPageBlockByIdQueryBuilder GetById(int customEntityVersionPageBlockId)
    {
        return new ContentRepositoryCustomEntityPageBlockByIdQueryBuilder(ExtendableContentRepository, customEntityVersionPageBlockId);
    }

    public async Task<int> AddAsync(AddCustomEntityVersionPageBlockCommand command)
    {
        await ExtendableContentRepository.ExecuteCommandAsync(command);
        return command.CustomEntityVersionId;
    }

    public Task UpdateAsync(UpdateCustomEntityVersionPageBlockCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task UpdateAsync(int customEntityVersionPageBlockId, Action<UpdateCustomEntityVersionPageBlockCommand> commandPatcher)
    {
        return ExtendableContentRepository.PatchCommandAsync(customEntityVersionPageBlockId, commandPatcher);
    }

    public Task MoveAsync(MoveCustomEntityVersionPageBlockCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task DeleteAsync(int customEntityVersionPageBlockId)
    {
        var command = new DeleteCustomEntityVersionPageBlockCommand() { CustomEntityVersionPageBlockId = customEntityVersionPageBlockId };
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }
}
