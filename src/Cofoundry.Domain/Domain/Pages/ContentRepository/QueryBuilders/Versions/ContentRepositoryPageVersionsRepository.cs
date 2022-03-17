using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageVersionsRepository
        : IAdvancedContentRepositoryPageVersionsRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryPageVersionsRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IAdvancedContentRepositoryPageVersionsByPageIdQueryBuilder GetByPageId()
    {
        return new ContentRepositoryPageVersionsByPageIdQueryBuilder(ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<bool> HasDraft(int pageId)
    {
        var query = new DoesPageHaveDraftVersionQuery(pageId);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public async Task<int> AddDraftAsync(AddPageDraftVersionCommand command)
    {
        await ExtendableContentRepository.ExecuteCommandAsync(command);
        return command.OutputPageVersionId;
    }

    public Task UpdateDraftAsync(UpdatePageDraftVersionCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task UpdateDraftAsync(int pageId, Action<UpdatePageDraftVersionCommand> commandPatcher)
    {
        return ExtendableContentRepository.PatchCommandAsync(pageId, commandPatcher);
    }

    public Task DeleteDraftAsync(int pageId)
    {
        var command = new DeletePageDraftVersionCommand() { PageId = pageId };
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public IAdvancedContentRepositoryPageRegionsRepository Regions()
    {
        return new ContentRepositoryPageRegionsRepository(ExtendableContentRepository);
    }
}
