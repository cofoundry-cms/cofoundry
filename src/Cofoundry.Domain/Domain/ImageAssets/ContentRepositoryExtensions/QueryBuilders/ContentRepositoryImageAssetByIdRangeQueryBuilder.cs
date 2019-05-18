using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryImageAssetByIdRangeQueryBuilder
        : IContentRepositoryImageAssetByIdRangeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private IEnumerable<int> _imageAssetIds;

        public ContentRepositoryImageAssetByIdRangeQueryBuilder(
            IExtendableContentRepository contentRepository,
            IEnumerable<int> imageAssetIds
            )
        {
            ExtendableContentRepository = contentRepository;
            _imageAssetIds = imageAssetIds;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<IDictionary<int, ImageAssetRenderDetails>> AsRenderDetailsAsync()
        {
            var query = new GetImageAssetRenderDetailsByIdRangeQuery(_imageAssetIds);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
