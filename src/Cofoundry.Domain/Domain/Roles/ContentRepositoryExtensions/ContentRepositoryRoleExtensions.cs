using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for roles.
/// </summary>
public static class ContentRepositoryRoleExtensions
{
    extension(IContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to roles from the Cofoundry identity
        /// system. Roles are an assignable collection of permissions. Every user has to be assigned 
        /// to one role.
        /// </summary>
        public IContentRepositoryRoleRepository Roles()
        {
            return new ContentRepositoryRoleRepository(contentRepository.AsExtendableContentRepository());
        }
    }

    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to roles from the Cofoundry identity
        /// system. Roles are an assignable collection of permissions. Every user has to be assigned 
        /// to one role.
        /// </summary>
        public IAdvancedContentRepositoryRoleRepository Roles()
        {
            return new ContentRepositoryRoleRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
