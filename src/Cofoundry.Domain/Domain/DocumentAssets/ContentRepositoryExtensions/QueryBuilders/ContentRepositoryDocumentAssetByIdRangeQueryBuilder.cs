using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryDocumentAssetByIdRangeQueryBuilder
        : IContentRepositoryDocumentAssetByIdRangeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly IEnumerable<int> _documentAssetIds;

        public ContentRepositoryDocumentAssetByIdRangeQueryBuilder(
            IExtendableContentRepository contentRepository,
            IEnumerable<int> documentAssetIds
            )
        {
            ExtendableContentRepository = contentRepository;
            _documentAssetIds = documentAssetIds;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<IDictionary<int, DocumentAssetRenderDetails>> AsRenderDetailsAsync()
        {
            var query = new GetDocumentAssetRenderDetailsByIdRangeQuery(_documentAssetIds);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
