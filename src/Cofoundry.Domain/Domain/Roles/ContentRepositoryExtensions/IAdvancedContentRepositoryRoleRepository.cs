using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the Role entity.
    /// </summary>
    public interface IAdvancedContentRepositoryRoleRepository : IContentRepositoryRoleRepository
    {
        #region queries

        /// <summary>
        /// Seaches roles based on simple filter criteria.
        /// </summary>
        /// <param name="query">Search query parameters.</param>
        IContentRepositoryRoleSearchQueryBuilder Search();

        /// <summary>
        /// Query to determine if a role title is unique within a specific UserArea.
        /// Role titles only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters to run the query with.</param>
        IDomainRepositoryQueryContext<bool> IsRoleTitleUnique(IsRoleTitleUniqueQuery query);

        #endregion

        #region commands

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
        Task UpdateRoleAsync(UpdateRoleCommand command);

        /// <summary>
        /// Deletes a role with the specified database id. Roles cannot be
        /// deleted if assigned to users.
        /// </summary>
        /// <param name="roleId">RoleId of the role to delete.</param>
        Task DeleteRoleAsync(int roleId);

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

        #endregion

        #region child entities

        IAdvancedContentRepositoryPermissionsRepository Permissions();

        #endregion
    }
}
