using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryPageDirectoryExtensions
    {
        /// <summary>
        /// PageDirectories represent a folder in the dynamic web page hierarchy.
        /// </summary>
        public static IContentRepositoryPageDirectoryRepository PageDirectories(this IContentRepository contentRepository)
        {
            return new ContentRepositoryPageDirectoryRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// PageDirectories represent a folder in the dynamic web page hierarchy.
        /// </summary>
        public static IAdvancedContentRepositoryPageDirectoryRepository PageDirectories(this IAdvancedContentRepository contentRepository)
        {
            return new AdvancedContentRepositoryPageDirectoryRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
