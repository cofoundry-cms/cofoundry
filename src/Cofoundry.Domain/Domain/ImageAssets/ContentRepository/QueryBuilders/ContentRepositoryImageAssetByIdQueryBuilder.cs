using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryImageAssetByIdQueryBuilder
        : IContentRepositoryImageAssetByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _imageAssetId;

        public ContentRepositoryImageAssetByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int imageAssetId
            )
        {
            ExtendableContentRepository = contentRepository;
            _imageAssetId = imageAssetId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<ImageAssetRenderDetails> AsRenderDetails()
        {
            var query = new GetImageAssetRenderDetailsByIdQuery(_imageAssetId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
