using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class AdvancedContentRepositoryPageDirectoryRepository
        : IAdvancedContentRepositoryPageDirectoryRepository
        , IExtendableContentRepositoryPart
{
    public AdvancedContentRepositoryPageDirectoryRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IAdvancedContentRepositoryPageDirectoryByIdQueryBuilder GetById(int pageDirectoryId)
    {
        return new ContentRepositoryPageDirectoryByIdQueryBuilder(ExtendableContentRepository, pageDirectoryId);
    }

    public IAdvancedContentRepositoryPageDirectoryGetAllQueryBuilder GetAll()
    {
        return new ContentRepositoryPageDirectoryGetAllQueryBuilder(ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<bool> IsPathUnique(IsPageDirectoryPathUniqueQuery query)
    {
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public async Task<int> AddAsync(AddPageDirectoryCommand command)
    {
        await ExtendableContentRepository.ExecuteCommandAsync(command);
        return command.OutputPageDirectoryId;
    }

    public Task UpdateAsync(UpdatePageDirectoryCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task UpdateAsync(int pageDirectoryId, Action<UpdatePageDirectoryCommand> commandPatcher)
    {
        return ExtendableContentRepository.PatchCommandAsync(pageDirectoryId, commandPatcher);
    }

    public Task UpdateUrlAsync(UpdatePageDirectoryUrlCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task DeleteAsync(int pageDirectoryId)
    {
        var command = new DeletePageDirectoryCommand()
        {
            PageDirectoryId = pageDirectoryId
        };

        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public IAdvancedContentRepositoryPageDirectoryAccessRulesRepository AccessRules()
    {
        return new ContentRepositoryPageDirectoryAccessRulesRepository(ExtendableContentRepository);
    }
}
