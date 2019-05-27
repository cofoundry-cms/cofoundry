using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class AdvancedContentRepositoryDocumentAssetByIdQueryBuilder
        : IAdvancedContentRepositoryDocumentAssetByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _documentAssetId;

        public AdvancedContentRepositoryDocumentAssetByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int documentAssetId
            )
        {
            ExtendableContentRepository = contentRepository;
            _documentAssetId = documentAssetId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<DocumentAssetDetails> AsDetailsAsync()
        {
            var query = new GetDocumentAssetDetailsByIdQuery(_documentAssetId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<DocumentAssetFile> AsFileAsync()
        {
            var query = new GetDocumentAssetFileByIdQuery(_documentAssetId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<DocumentAssetRenderDetails> AsRenderDetailsAsync()
        {
            var query = new GetDocumentAssetRenderDetailsByIdQuery(_documentAssetId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
