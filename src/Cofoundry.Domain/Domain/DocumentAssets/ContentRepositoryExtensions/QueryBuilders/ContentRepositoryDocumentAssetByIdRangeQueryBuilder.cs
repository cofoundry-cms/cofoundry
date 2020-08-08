using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<IDictionary<int, DocumentAssetRenderDetails>> AsRenderDetails()
        {
            var query = new GetDocumentAssetRenderDetailsByIdRangeQuery(_documentAssetIds);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
