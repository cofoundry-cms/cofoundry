using Cofoundry.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class AnonymousRolePermissionInitializer : IRolePermissionInitializer
    {
        private readonly IServiceProvider _serviceProvider;

        public AnonymousRolePermissionInitializer(
            IServiceProvider serviceProvider
            )
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public void Initialize(IPermissionSetBuilder permissionSetBuilder)
        {
            if (permissionSetBuilder == null) throw new ArgumentNullException(nameof(permissionSetBuilder));

            // A custom IAnonymousRolePermissionConfiguration implementation can optionally be defined
            // which overrides the base implementation in the definition.
            var anonymousRolePermissionConfiguration = _serviceProvider.GetService<IEnumerable<IAnonymousRolePermissionConfiguration>>();
            if (EnumerableHelper.IsNullOrEmpty(anonymousRolePermissionConfiguration))
            {
                var anonymousRole = new AnonymousRole();
                anonymousRole.ConfigurePermissions(permissionSetBuilder);
            }
            else if (anonymousRolePermissionConfiguration.Count() > 1)
            {
                throw new InvalidOperationException($"Expected a single implementation of {nameof(IAnonymousRolePermissionConfiguration)} but encountered {anonymousRolePermissionConfiguration.Count()}. Only one implementation is permitted.");
            }
            else
            {
                anonymousRolePermissionConfiguration
                    .First()
                    .ConfigurePermissions(permissionSetBuilder);
            }
        }
    }
}