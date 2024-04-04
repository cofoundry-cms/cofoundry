namespace Cofoundry.Domain;

/// <summary>
/// <see cref="IAdvancedContentRepository"/> extension root for the Role entity.
/// </summary>
public interface IAdvancedContentRepositoryRoleRepository : IContentRepositoryRoleRepository
{
    /// <summary>
    /// Seaches roles based on simple filter criteria.
    /// </summary>
    IContentRepositoryRoleSearchQueryBuilder Search();

    /// <summary>
    /// Query to determine if a role title is unique within a specific UserArea.
    /// Role titles only have to be unique per UserArea.
    /// </summary>
    /// <param name="query">The parameters to run the query with.</param>
    IDomainRepositoryQueryContext<bool> IsTitleUnique(IsRoleTitleUniqueQuery query);

    /// <summary>
    /// Adds a new role to a user area with a specific set of permissions.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    /// <returns>Id of the newly created role.</returns>
    Task<int> AddAsync(AddRoleCommand command);

    /// <summary>
    /// Updates an existing role. Also updates the role permission set.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    Task UpdateAsync(UpdateRoleCommand command);

    /// <summary>
    /// Updates an existing role. Also updates the role permission set.
    /// </summary>
    /// <param name="roleId">
    /// The database id of the role to update.
    /// </param>
    /// <param name="commandPatcher">
    /// An action to configure or "patch" a command that's been initialized
    /// with the existing role data.
    /// </param>
    Task UpdateAsync(int roleId, Action<UpdateRoleCommand> commandPatcher);

    /// <summary>
    /// Deletes a role with the specified database id. Roles cannot be
    /// deleted if assigned to users.
    /// </summary>
    /// <param name="roleId">RoleId of the role to delete.</param>
    Task DeleteAsync(int roleId);

    /// <summary>
    /// Registers new roles defined in code via IRoleDefinition and initializes
    /// permissions when an IRoleInitializer has been implemented. You do not
    /// normally need to run this because it is run automatically during the
    /// startup process, however you can run this command manually with the 
    /// UpdateExistingRoles flag to force an update of existing roles.
    /// </summary>
    /// <param name="command">
    /// Command parameters. 
    /// By default we don't update roles once they
    /// are in the system because we don't want to overwrite
    /// changes made in the UI, but you can force the update
    /// by running this command manually with the UpdateExistingRoles 
    /// parameter set to true.
    /// </param>
    Task RegisterPermissionsAndRoles(RegisterPermissionsAndRolesCommand command);

    IAdvancedContentRepositoryPermissionsRepository Permissions();
}
