using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryDocumentAssetByIdQueryBuilder
        : IContentRepositoryDocumentAssetByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _documentAssetId;

        public ContentRepositoryDocumentAssetByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int documentAssetId
            )
        {
            ExtendableContentRepository = contentRepository;
            _documentAssetId = documentAssetId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<DocumentAssetRenderDetails> AsRenderDetailsAsync()
        {
            var query = new GetDocumentAssetRenderDetailsByIdQuery(_documentAssetId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
