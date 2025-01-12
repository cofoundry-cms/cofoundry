﻿namespace Cofoundry.Domain.Tests;

/// <summary>
/// Used to make it easier to create roles in test fixtures.
/// </summary>
public class RoleTestDataHelper
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPermissionRepository _permissionRepository;

    public RoleTestDataHelper(
        IServiceProvider serviceProvider,
        IPermissionRepository permissionRepository
        )
    {
        _serviceProvider = serviceProvider;
        _permissionRepository = permissionRepository;
    }

    /// <summary>
    /// Adds a new role.
    /// </summary>
    /// <param name="uniqueData">Unique data to use in the "Title" property.</param>
    /// <param name="userAreaCode">User area to assign the role to.</param>
    /// <param name="permissionInitializer">
    /// A filter function to indicate which permissions to add. If this is <see langword="null"/> 
    /// then all permissions are added. This is the equivalent of an <see cref="IRoleInitializer"/>.
    /// </param>
    public async Task<int> AddAsync(
        string uniqueData,
        string userAreaCode,
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? permissionInitializer = null
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
    /// then all permissions are added. This is the equivalent of an <see cref="IRoleInitializer"/>.
    /// </param>
    public AddRoleCommand CreateAddCommand(string uniqueData, string userAreaCode, Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? permissionInitializer = null)
    {
        var command = new AddRoleCommand()
        {
            Title = uniqueData,
            UserAreaCode = userAreaCode
        };

        var permissions = _permissionRepository.GetAll();

        if (permissionInitializer != null)
        {
            permissions = permissionInitializer(permissions);
        }

        command.Permissions = permissions
            .Select(p => new PermissionCommandData()
            {
                EntityDefinitionCode = (p as IEntityPermission)?.EntityDefinition.EntityDefinitionCode,
                PermissionCode = p.PermissionType.Code
            })
            .ToList();

        return command;
    }
}
