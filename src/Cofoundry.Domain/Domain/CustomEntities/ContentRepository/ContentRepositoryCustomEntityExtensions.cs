using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for custom entities.
/// </summary>
public static class ContentRepositoryCustomEntityExtensions
{
    extension(IContentRepository contentRepository)
    {
        /// <summary>
        /// Custom entities are a flexible system for developer defined
        /// data structures which can be fully managed in the admin panel 
        /// with minimal configuration.
        /// </summary>
        public IContentRepositoryCustomEntityRepository CustomEntities()
        {
            return new ContentRepositoryCustomEntityRepository(contentRepository.AsExtendableContentRepository());
        }
    }

    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Custom entities are a flexible system for developer defined
        /// data structures which can be fully managed in the admin panel 
        /// with minimal configuration.
        /// </summary>
        public IAdvancedContentRepositoryCustomEntityRepository CustomEntities()
        {
            return new AdvancedContentRepositoryCustomEntityRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
