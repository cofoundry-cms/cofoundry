using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for users.
/// </summary>
public static class ContentRepositoryUserExtensions
{
    extension(IContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to users from the Cofoundry identity
        /// system. This includes users from both the Cofoundry admin user area
        /// and any custom user areas.
        /// </summary>
        public IContentRepositoryUserRepository Users()
        {
            return new ContentRepositoryUserRepository(contentRepository.AsExtendableContentRepository());
        }
    }

    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to users from the Cofoundry identity
        /// system. This includes users from both the Cofoundry admin user area
        /// and any custom user areas.
        /// </summary>
        public IAdvancedContentRepositoryUserRepository Users()
        {
            return new AdvancedContentRepositoryUserRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
