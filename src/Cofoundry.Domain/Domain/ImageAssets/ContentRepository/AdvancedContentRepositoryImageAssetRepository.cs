using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class AdvancedContentRepositoryImageAssetRepository
        : IAdvancedContentRepositoryImageAssetRepository
        , IExtendableContentRepositoryPart
{
    public AdvancedContentRepositoryImageAssetRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IAdvancedContentRepositoryImageAssetByIdQueryBuilder GetById(int imageAssetId)
    {
        return new AdvancedContentRepositoryImageAssetByIdQueryBuilder(ExtendableContentRepository, imageAssetId);
    }

    public IContentRepositoryImageAssetByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> imageAssetIds)
    {
        return new ContentRepositoryImageAssetByIdRangeQueryBuilder(ExtendableContentRepository, imageAssetIds);
    }

    public IContentRepositoryImageAssetSearchQueryBuilder Search()
    {
        return new ContentRepositoryImageAssetSearchQueryBuilder(ExtendableContentRepository);
    }

    public async Task<int> AddAsync(AddImageAssetCommand command)
    {
        await ExtendableContentRepository.ExecuteCommandAsync(command);
        return command.OutputImageAssetId;
    }

    public Task UpdateAsync(UpdateImageAssetCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task UpdateAsync(int imageAssetId, Action<UpdateImageAssetCommand> commandPatcher)
    {
        return ExtendableContentRepository.PatchCommandAsync(imageAssetId, commandPatcher);
    }

    public Task DeleteAsync(int imageAssetId)
    {
        var command = new DeleteImageAssetCommand()
        {
            ImageAssetId = imageAssetId
        };

        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }
}
