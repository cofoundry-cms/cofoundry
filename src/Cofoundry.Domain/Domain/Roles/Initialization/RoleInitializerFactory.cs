using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Factory for creating role initializers to avoid exposing IResolutionContext
    /// directly.
    /// </summary>
    public class RoleInitializerFactory : IRoleInitializerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RoleInitializerFactory(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates an instance of a role initializer for the specified role
        /// definition if one has been implemented; otherwise returns null.
        /// </summary>
        /// <param name="roleDefinition">The role to find an initializer for.</param>
        /// <returns>IRoleInitializer if one has been implemented; otherwise null.</returns>
        public virtual IRoleInitializer Create(IRoleDefinition roleDefinition)
        {
            if (roleDefinition == null) throw new ArgumentNullException(nameof(roleDefinition));

            // Never add permission to the super admin role
            if (roleDefinition is SuperAdminRole) return null;

            var roleInitializerType = typeof(IRoleInitializer<>).MakeGenericType(roleDefinition.GetType());
            var roleInitializer = _serviceProvider.GetService(roleInitializerType);
            if (roleInitializer != null)
            {
                return (IRoleInitializer)roleInitializer;
            }
            else if (roleDefinition is AnonymousRole)
            {
                // We use a default initializer just for the anonymous role
                // as it's the only built in role with permissions
                return new DefaultAnonymousRoleInitializer();
            }

            return null;
        }
    }
}
