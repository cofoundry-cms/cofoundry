using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Used to make it easier to create roles in test fixtures.
    /// </summary>
    public class RoleTestDataHelper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SeededEntities _seededEntities;
        private readonly IPermissionRepository _permissionRepository;

        public RoleTestDataHelper(
            IServiceProvider serviceProvider,
            SeededEntities seededEntities,
            IPermissionRepository permissionRepository
            )
        {
            _serviceProvider = serviceProvider;
            _seededEntities = seededEntities;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// Adds a new role.
        /// </summary>
        /// <param name="uniqueData">Unique data to use in the "Title" property.</param>
        /// <param name="userAreaCode">User area to assign the role to.</param>
        /// <param name="permissionInitializer">
        /// A filter function to indicate which permissions to add. If this is <see langword="null"/> 
        /// then no permissions are added. This is the equivalent of an <see cref="IRoleInitializer"/>.
        /// </param>
        public async Task<int> AddAsync(
            string uniqueData,
            string userAreaCode,
            Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionInitializer = null
            )
        {
            var command = CreateAddCommand(uniqueData, userAreaCode, permissionInitializer);

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope.ServiceProvider.GetRequiredService<IAdvancedContentRepository>();

            return await contentRepository
                .WithElevatedPermissions()
                .Roles()
                .AddAsync(command);
        }

        /// <summary>
        /// Creates a valid <see cref="AddRoleCommand"/>.
        /// </summary>
        /// <param name="uniqueData">Unique data to use in the "Title" property.</param>
        /// <param name="userAreaCode">User area to assign the role to.</param>
        /// <param name="permissionInitializer">
        /// A filter function to indicate which permissions to add. If this is <see langword="null"/> 
        /// then no permissions are added. This is the equivalent of an <see cref="IRoleInitializer"/>.
        /// </param>
        public AddRoleCommand CreateAddCommand(string uniqueData, string userAreaCode, Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionInitializer = null)
        {
            var command = new AddRoleCommand()
            {
                Title = uniqueData,
                UserAreaCode = userAreaCode
            };

            if (permissionInitializer != null)
            {
                var permissions = _permissionRepository.GetAll();
                command.Permissions = permissionInitializer(permissions)
                    .Select(p => new PermissionCommandData()
                    {
                        EntityDefinitionCode = (p as IEntityPermission)?.EntityDefinition.EntityDefinitionCode,
                        PermissionCode = p.PermissionType.Code
                    })
                    .ToList();
            }

            return command;
        }
    }
}
