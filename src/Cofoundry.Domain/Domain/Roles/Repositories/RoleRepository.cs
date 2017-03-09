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
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<IEnumerable<IPermission>> GetAllPermissionsAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<IPermission>(executionContext);
        }

        /// <summary>
        /// Finds a role by it's database id, returning a RoleDetails object if it 
        /// is found, otherwise null. If no role id is specified then the anonymous 
        /// role is returned.
        /// </summary>
        /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public RoleDetails GetRoleDetailsById(int? roleId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetRoleDetailsByIdQuery(roleId), executionContext);
        }

        /// <summary>
        /// Finds a role by it's database id, returning a RoleDetails object if it 
        /// is found, otherwise null. If no role id is specified then the anonymous 
        /// role is returned.
        /// </summary>
        /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<RoleDetails> GetRoleDetailsByIdAsync(int? roleId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(roleId), executionContext);
        }

        /// <summary>
        /// Find a role with the specified specialist role type code, returning
        /// a RoleDetails object if one is found, otherwise null. Roles only
        /// have a SpecialistRoleTypeCode if they have been generated from code
        /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
        /// </summary>
        /// <param name="specialistRoleTypeCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public RoleDetails GetRoleDetailsBySpecialistRoleTypeCode(string specialistRoleTypeCode, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetRoleDetailsBySpecialistRoleTypeCode(specialistRoleTypeCode), executionContext);
        }

        /// <summary>
        /// Find a role with the specified specialist role type code, returning
        /// a RoleDetails object if one is found, otherwise null. Roles only
        /// have a SpecialistRoleTypeCode if they have been generated from code
        /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
        /// </summary>
        /// <param name="specialistRoleTypeCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<RoleDetails> GetRoleDetailsBySpecialistRoleTypeCodeAsync(string specialistRoleTypeCode, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetRoleDetailsBySpecialistRoleTypeCode(specialistRoleTypeCode), executionContext);
        }

        /// <summary>
        /// Determines if a role title is unique within a specific UserArea.
        /// Role titles only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        /// <returns>True if the title is unique; otherwise false.</returns>
        public bool IsRoleTitleUnique(IsRoleTitleUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        /// <summary>
        /// Seaches roles based on simple filter criteria and returns a paged result. 
        /// </summary>
        /// <param name="query">Search query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<PagedQueryResult<RoleMicroSummary>> SearchRolesAsync(SearchRolesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region commands

        /// <summary>
        /// Adds a new role to a user area with a specific set of permissions.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task AddRoleAsync(AddRoleCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        /// <summary>
        /// Updates an existing role. Also updates the role permission set.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task UpdateRoleAsync(UpdateRoleCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        /// <summary>
        /// Deletes a role with the specified database id.
        /// </summary>
        /// <param name="roleId">Id of the role to delete.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task DeleteRoleAsync(int roleId, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(new DeleteRoleCommand(roleId), executionContext);
        }

        /// <summary>
        /// Registers new roles defined in code via IRoleDefinition and initializes
        /// permissions when an IRoleInitializer has been implemented.
        /// </summary>
        /// <param name="updateExistingRoles">
        /// By default we don't update roles once they
        /// are in the system because we don't want to overwrite
        /// changes made in the UI, but you can force the update
        /// by running this command manually with this flag.
        /// </param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task RegisterDefinedRoles(bool updateExistingRoles, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(new RegisterDefinedRolesCommand() { UpdateExistingRoles = updateExistingRoles }, executionContext);
        }

        #endregion
    }
}
