using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryUserExtensions
    {
        /// <summary>
        /// Queries and commands relating to users from the Cofoundry identity
        /// system. This includes users from both the Cofoundry admin user area
        /// and any custom user areas.
        /// </summary>
        public static IContentRepositoryUserRepository Users(this IContentRepository contentRepository)
        {
            return new ContentRepositoryUserRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// Queries and commands relating to users from the Cofoundry identity
        /// system. This includes users from both the Cofoundry admin user area
        /// and any custom user areas.
        /// </summary>
        public static IAdvancedContentRepositoryUserRepository Users(this IAdvancedContentRepository contentRepository)
        {
            return new AdvancedContentRepositoryUserRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
