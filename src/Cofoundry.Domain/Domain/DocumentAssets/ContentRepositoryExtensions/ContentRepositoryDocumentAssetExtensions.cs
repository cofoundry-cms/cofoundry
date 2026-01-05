using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for document assets.
/// </summary>
public static class ContentRepositoryDocumentAssetExtensions
{
    extension(IContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to document assets.
        /// </summary>
        public IContentRepositoryDocumentAssetRepository DocumentAssets()
        {
            return new ContentRepositoryDocumentAssetRepository(contentRepository.AsExtendableContentRepository());
        }
    }

    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to document assets.
        /// </summary>
        public IAdvancedContentRepositoryDocumentAssetRepository DocumentAssets()
        {
            return new AdvancedContentRepositoryDocumentAssetRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
