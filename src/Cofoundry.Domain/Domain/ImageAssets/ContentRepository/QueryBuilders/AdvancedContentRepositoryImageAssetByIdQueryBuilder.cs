using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AdvancedContentRepositoryImageAssetByIdQueryBuilder
        : IAdvancedContentRepositoryImageAssetByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _imageAssetId;

        public AdvancedContentRepositoryImageAssetByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int imageAssetId
            )
        {
            ExtendableContentRepository = contentRepository;
            _imageAssetId = imageAssetId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<ImageAssetDetails> AsDetails()
        {
            var query = new GetImageAssetDetailsByIdQuery(_imageAssetId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<ImageAssetFile> AsFile()
        {
            var query = new GetImageAssetFileByIdQuery(_imageAssetId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<ImageAssetRenderDetails> AsRenderDetails()
        {
            var query = new GetImageAssetRenderDetailsByIdQuery(_imageAssetId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
