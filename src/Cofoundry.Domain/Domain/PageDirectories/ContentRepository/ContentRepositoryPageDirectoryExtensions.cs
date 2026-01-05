using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for page directories.
/// </summary>
public static class ContentRepositoryPageDirectoryExtensions
{
    extension(IContentRepository contentRepository)
    {
        /// <summary>
        /// PageDirectories represent a folder in the dynamic web page hierarchy.
        /// </summary>
        public IContentRepositoryPageDirectoryRepository PageDirectories()
        {
            return new ContentRepositoryPageDirectoryRepository(contentRepository.AsExtendableContentRepository());
        }
    }

    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// PageDirectories represent a folder in the dynamic web page hierarchy.
        /// </summary>
        public IAdvancedContentRepositoryPageDirectoryRepository PageDirectories()
        {
            return new AdvancedContentRepositoryPageDirectoryRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
