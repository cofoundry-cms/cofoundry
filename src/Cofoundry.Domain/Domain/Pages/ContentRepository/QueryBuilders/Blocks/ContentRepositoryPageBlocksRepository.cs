using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageBlocksRepository
        : IAdvancedContentRepositoryPageBlocksRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryPageBlocksRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IAdvancedContentRepositoryPageBlockByIdQueryBuilder GetById(int pageVersionBlockId)
    {
        return new ContentRepositoryPageBlockByIdQueryBuilder(ExtendableContentRepository, pageVersionBlockId);
    }

    public async Task<int> AddAsync(AddPageVersionBlockCommand command)
    {
        await ExtendableContentRepository.ExecuteCommandAsync(command);
        return command.OutputPageBlockId;
    }

    public Task UpdateAsync(UpdatePageVersionBlockCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task UpdateAsync(int pageVersionBlockId, Action<UpdatePageVersionBlockCommand> commandPatcher)
    {
        return ExtendableContentRepository.PatchCommandAsync(pageVersionBlockId, commandPatcher);
    }

    public Task MoveAsync(MovePageVersionBlockCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task DeleteAsync(int pageVersionBlockId)
    {
        var command = new DeletePageVersionBlockCommand() { PageVersionBlockId = pageVersionBlockId };
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }
}
