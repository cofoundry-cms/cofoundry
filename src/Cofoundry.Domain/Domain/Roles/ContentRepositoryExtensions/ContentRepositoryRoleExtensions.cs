using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryRoleExtensions
    {
        /// <summary>
        /// Queries and commands relating to roles from the Cofoundry identity
        /// system. Roles are an assignable collection of permissions. Every user has to be assigned 
        /// to one role.
        /// </summary>
        public static IContentRepositoryRoleRepository Roles(this IContentRepository contentRepository)
        {
            return new ContentRepositoryRoleRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// Queries and commands relating to roles from the Cofoundry identity
        /// system. Roles are an assignable collection of permissions. Every user has to be assigned 
        /// to one role.
        /// </summary>
        public static IAdvancedContentRepositoryRoleRepository Roles(this IAdvancedContentRepository contentRepository)
        {
            return new ContentRepositoryRoleRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
