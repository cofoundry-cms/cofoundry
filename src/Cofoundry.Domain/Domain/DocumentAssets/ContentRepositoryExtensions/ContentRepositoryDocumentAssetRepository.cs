using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryDocumentAssetRepository
        : IContentRepositoryDocumentAssetRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryDocumentAssetRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IContentRepositoryDocumentAssetByIdQueryBuilder GetById(int documentAssetId)
    {
        return new ContentRepositoryDocumentAssetByIdQueryBuilder(ExtendableContentRepository, documentAssetId);
    }

    public IContentRepositoryDocumentAssetByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> documentAssetIds)
    {
        return new ContentRepositoryDocumentAssetByIdRangeQueryBuilder(ExtendableContentRepository, documentAssetIds);
    }

    public IContentRepositoryDocumentAssetSearchQueryBuilder Search()
    {
        return new ContentRepositoryDocumentAssetSearchQueryBuilder(ExtendableContentRepository);
    }

    public Task AddAsync(AddDocumentAssetCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task UpdateAsync(UpdateDocumentAssetCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task DeleteAsync(int documentAssetId)
    {
        var command = new DeleteDocumentAssetCommand()
        {
            DocumentAssetId = documentAssetId
        };

        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }
}