using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for image assets.
/// </summary>
public static class ContentRepositoryImageAssetExtensions
{
    extension(IContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to image assets.
        /// </summary>
        public IContentRepositoryImageAssetRepository ImageAssets()
        {
            return new ContentRepositoryImageAssetRepository(contentRepository.AsExtendableContentRepository());
        }
    }

    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to image assets.
        /// </summary>
        public IAdvancedContentRepositoryImageAssetRepository ImageAssets()
        {
            return new AdvancedContentRepositoryImageAssetRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
