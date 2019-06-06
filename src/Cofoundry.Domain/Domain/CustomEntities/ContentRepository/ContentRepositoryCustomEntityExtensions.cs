using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryCustomEntityExtensions
    {
        /// <summary>
        /// Queries and commands relating to custom entities.
        /// </summary>
        public static IContentRepositoryCustomEntityRepository CustomEntities(this IContentRepository contentRepository)
        {
            return new ContentRepositoryCustomEntityRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// Queries and commands relating to custom entities.
        /// </summary>
        public static IAdvancedContentRepositoryCustomEntityRepository CustomEntities(this IAdvancedContentRepository contentRepository)
        {
            return new AdvancedContentRepositoryCustomEntityRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
