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
    public interface IRoleInitializerFactory
    {
        /// <summary>
        /// Creates an instance of a role initializer for the specified role
        /// definition if one has been implemented; otherwise returns null.
        /// </summary>
        /// <param name="roleDefinition">The role to find an initializer for.</param>
        /// <returns>IRoleInitializer if one has been implemented; otherwise null.</returns>
        IRoleInitializer Create(IRoleDefinition roleDefinition);
    }
}
