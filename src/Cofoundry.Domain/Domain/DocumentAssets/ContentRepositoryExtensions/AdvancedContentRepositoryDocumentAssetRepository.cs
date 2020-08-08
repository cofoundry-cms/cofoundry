using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AdvancedContentRepositoryDocumentAssetRepository
            : IAdvancedContentRepositoryDocumentAssetRepository
            , IExtendableContentRepositoryPart
    {
        public AdvancedContentRepositoryDocumentAssetRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IAdvancedContentRepositoryDocumentAssetByIdQueryBuilder GetById(int documentAssetId)
        {
            return new AdvancedContentRepositoryDocumentAssetByIdQueryBuilder(ExtendableContentRepository, documentAssetId);
        }

        public IContentRepositoryDocumentAssetByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> documentAssetIds)
        {
            return new ContentRepositoryDocumentAssetByIdRangeQueryBuilder(ExtendableContentRepository, documentAssetIds);
        }

        public IContentRepositoryDocumentAssetSearchQueryBuilder Search()
        {
            return new ContentRepositoryDocumentAssetSearchQueryBuilder(ExtendableContentRepository);
        }

        #endregion

        #region commands

        public async Task<int> AddAsync(AddDocumentAssetCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputDocumentAssetId;
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

        #endregion
    }
}
