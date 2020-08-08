using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryImageAssetExtensions
    {
        /// <summary>
        /// Queries and commands relating to image assets.
        /// </summary>
        public static IContentRepositoryImageAssetRepository ImageAssets(this IContentRepository contentRepository)
        {
            return new ContentRepositoryImageAssetRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// Queries and commands relating to image assets.
        /// </summary>
        public static IAdvancedContentRepositoryImageAssetRepository ImageAssets(this IAdvancedContentRepository contentRepository)
        {
            return new AdvancedContentRepositoryImageAssetRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
