using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryPageExtensions
    {
        /// <summary>
        /// Queries and commands relating to pages.
        /// </summary>
        public static IContentRepositoryPageRepository Pages(this IContentRepository contentRepository)
        {
            return new ContentRepositoryPageRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// Queries and commands relating to pages.
        /// </summary>
        public static IAdvancedContentRepositoryPageRepository Pages(this IAdvancedContentRepository contentRepository)
        {
            return new AdvancedContentRepositoryPageRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
