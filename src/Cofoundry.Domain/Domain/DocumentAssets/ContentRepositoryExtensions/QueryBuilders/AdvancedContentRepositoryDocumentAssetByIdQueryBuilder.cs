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

        public IContentRepositoryQueryContext<DocumentAssetDetails> AsDetails()
        {
            var query = new GetDocumentAssetDetailsByIdQuery(_documentAssetId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<DocumentAssetFile> AsFile()
        {
            var query = new GetDocumentAssetFileByIdQuery(_documentAssetId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<DocumentAssetRenderDetails> AsRenderDetails()
        {
            var query = new GetDocumentAssetRenderDetailsByIdQuery(_documentAssetId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
