using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over role data access queries/commands to them more discoverable.
    /// </summary>
    public interface IRoleRepository
    {
        #region queries

        /// <summary>
        /// Returns all IPermission instances registered with Cofoundry.
        /// </summary>
        Task<IEnumerable<IPermission>> GetAllPermissionsAsync();

        /// <summary>
        /// Finds a role by it's database id, returning a RoleDetails object if it 
        /// is found, otherwise null. If no role id is specified then the anonymous 
        /// role is returned.
        /// </summary>
        /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
        RoleDetails GetRoleDetailsById(int? roleId);

        /// <summary>
        /// Finds a role by it's database id, returning a RoleDetails object if it 
        /// is found, otherwise null. If no role id is specified then the anonymous 
        /// role is returned.
        /// </summary>
        /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
        Task<RoleDetails> GetRoleDetailsByIdAsync(int? roleId);

        /// <summary>
        /// Find a role with the specified specialist role type code, returning
        /// a RoleDetails object if one is found, otherwise null. Roles only
        /// have a SpecialistRoleTypeCode if they have been generated from code
        /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
        /// </summary>
        /// <param name="specialistRoleTypeCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        RoleDetails GetRoleDetailsBySpecialistRoleTypeCode(string specialistRoleTypeCode);

        /// <summary>
        /// Find a role with the specified specialist role type code, returning
        /// a RoleDetails object if one is found, otherwise null. Roles only
        /// have a SpecialistRoleTypeCode if they have been generated from code
        /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
        /// </summary>
        /// <param name="specialistRoleTypeCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        Task<RoleDetails> GetRoleDetailsBySpecialistRoleTypeCodeAsync(string specialistRoleTypeCode);

        /// <summary>
        /// Determines if a role title is unique within a specific UserArea.
        /// Role titles only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <returns>True if the title is unique; otherwise false.</returns>
        bool IsRoleTitleUnique(IsRoleTitleUniqueQuery query);

        /// <summary>
        /// Seaches roles based on simple filter criteria and returns a paged result. 
        /// </summary>
        /// <param name="query">Search query parameters.</param>
        Task<PagedQueryResult<RoleMicroSummary>> SearchRolesAsync(SearchRolesQuery query);

        #endregion

        #region commands

        /// <summary>
        /// Adds a new role to a user area with a specific set of permissions.
        /// </summary>
        Task AddRoleAsync(AddRoleCommand command);

        /// <summary>
        /// Updates an existing role. Also updates the role permission set.
        /// </summary>
        Task UpdateRoleAsync(UpdateRoleCommand command);

        /// <summary>
        /// Deletes a role with the specified database id.
        /// </summary>
        /// <param name="roleId">Id of the role to delete.</param>
        Task DeleteRoleAsync(int roleId);

        /// <summary>
        /// Registers new roles defined in code via IRoleDefinition and initializes
        /// permissions when an IRoleInitializer has been implemented.
        /// </summary>
        Task RegisterDefinedRoles(bool updateExistingRoles = false);

        #endregion
    }
}
