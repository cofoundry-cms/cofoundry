using Cofoundry.Domain.CQS;
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
    public class RoleRepository : IRoleRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public RoleRepository(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region queries

        /// <summary>
        /// Returns all IPermission instances registered with Cofoundry.
        /// </summary>
        public Task<IEnumerable<IPermission>> GetAllPermissionsAsync()
        {
            return _queryExecutor.GetAllAsync<IPermission>();
        }

        /// <summary>
        /// Finds a role by it's database id, returning a RoleDetails object if it 
        /// is found, otherwise null. If no role id is specified then the anonymous 
        /// role is returned.
        /// </summary>
        /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
        public RoleDetails GetRoleDetailsById(int? roleId)
        {
            return _queryExecutor.Execute(new GetRoleDetailsByIdQuery(roleId));
        }

        /// <summary>
        /// Finds a role by it's database id, returning a RoleDetails object if it 
        /// is found, otherwise null. If no role id is specified then the anonymous 
        /// role is returned.
        /// </summary>
        /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
        public Task<RoleDetails> GetRoleDetailsByIdAsync(int? roleId)
        {
            return _queryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(roleId));
        }

        /// <summary>
        /// Find a role with the specified specialist role type code, returning
        /// a RoleDetails object if one is found, otherwise null. Roles only
        /// have a SpecialistRoleTypeCode if they have been generated from code
        /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
        /// </summary>
        /// <param name="specialistRoleTypeCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        public RoleDetails GetRoleDetailsBySpecialistRoleTypeCode(string specialistRoleTypeCode)
        {
            return _queryExecutor.Execute(new GetRoleDetailsBySpecialistRoleTypeCode(specialistRoleTypeCode));
        }

        /// <summary>
        /// Find a role with the specified specialist role type code, returning
        /// a RoleDetails object if one is found, otherwise null. Roles only
        /// have a SpecialistRoleTypeCode if they have been generated from code
        /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
        /// </summary>
        /// <param name="specialistRoleTypeCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        public Task<RoleDetails> GetRoleDetailsBySpecialistRoleTypeCodeAsync(string specialistRoleTypeCode)
        {
            return _queryExecutor.ExecuteAsync(new GetRoleDetailsBySpecialistRoleTypeCode(specialistRoleTypeCode));
        }

        /// <summary>
        /// Determines if a role title is unique within a specific UserArea.
        /// Role titles only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <returns>True if the title is unique; otherwise false.</returns>
        public bool IsRoleTitleUnique(IsRoleTitleUniqueQuery query)
        {
            return _queryExecutor.Execute(query);
        }

        /// <summary>
        /// Seaches roles based on simple filter criteria and returns a paged result. 
        /// </summary>
        /// <param name="query">Search query parameters.</param>
        public Task<PagedQueryResult<RoleMicroSummary>> SearchRolesAsync(SearchRolesQuery query)
        {
            return _queryExecutor.ExecuteAsync(query);
        }

        #endregion

        #region commands

        /// <summary>
        /// Adds a new role to a user area with a specific set of permissions.
        /// </summary>
        public Task AddRoleAsync(AddRoleCommand command)
        {
            return _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// Updates an existing role. Also updates the role permission set.
        /// </summary>
        public Task UpdateRoleAsync(UpdateRoleCommand command)
        {
            return _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// Deletes a role with the specified database id.
        /// </summary>
        /// <param name="roleId">Id of the role to delete.</param>
        public Task DeleteRoleAsync(int roleId)
        {
            return _commandExecutor.ExecuteAsync(new DeleteRoleCommand(roleId));
        }

        /// <summary>
        /// Registers new roles defined in code via IRoleDefinition and initializes
        /// permissions when an IRoleInitializer has been implemented.
        /// </summary>
        public Task RegisterDefinedRoles(bool updateExistingRoles = false)
        {
            return _commandExecutor.ExecuteAsync(new RegisterDefinedRolesCommand() { UpdateExistingRoles = updateExistingRoles });
        }

        #endregion
    }
}
