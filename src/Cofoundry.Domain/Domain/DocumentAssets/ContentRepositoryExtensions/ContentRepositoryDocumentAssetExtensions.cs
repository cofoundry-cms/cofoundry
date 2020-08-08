using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryDocumentAssetExtensions
    {
        /// <summary>
        /// Queries and commands relating to document assets.
        /// </summary>
        public static IContentRepositoryDocumentAssetRepository DocumentAssets(this IContentRepository contentRepository)
        {
            return new ContentRepositoryDocumentAssetRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// Queries and commands relating to document assets.
        /// </summary>
        public static IAdvancedContentRepositoryDocumentAssetRepository DocumentAssets(this IAdvancedContentRepository contentRepository)
        {
            return new AdvancedContentRepositoryDocumentAssetRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
