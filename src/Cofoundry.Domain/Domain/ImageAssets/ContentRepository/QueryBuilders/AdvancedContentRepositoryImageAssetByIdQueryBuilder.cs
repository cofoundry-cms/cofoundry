using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public Task<ImageAssetDetails> AsDetailsAsync()
        {
            var query = new GetImageAssetDetailsByIdQuery(_imageAssetId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<ImageAssetFile> AsFileAsync()
        {
            var query = new GetImageAssetFileByIdQuery(_imageAssetId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<ImageAssetRenderDetails> AsRenderDetailsAsync()
        {
            var query = new GetImageAssetRenderDetailsByIdQuery(_imageAssetId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
