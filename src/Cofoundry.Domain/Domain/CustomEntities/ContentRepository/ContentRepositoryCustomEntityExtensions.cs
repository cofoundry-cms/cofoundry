using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryCustomEntityExtensions
    {
        /// <summary>
        /// Custom entities are a flexible system for developer defined
        /// data structures which can be fully managed in the admin panel 
        /// with minimal configuration.
        /// </summary>
        public static IContentRepositoryCustomEntityRepository CustomEntities(this IContentRepository contentRepository)
        {
            return new ContentRepositoryCustomEntityRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// Custom entities are a flexible system for developer defined
        /// data structures which can be fully managed in the admin panel 
        /// with minimal configuration.
        /// </summary>
        public static IAdvancedContentRepositoryCustomEntityRepository CustomEntities(this IAdvancedContentRepository contentRepository)
        {
            return new AdvancedContentRepositoryCustomEntityRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
