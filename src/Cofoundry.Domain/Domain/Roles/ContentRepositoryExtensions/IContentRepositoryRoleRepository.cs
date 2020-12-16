using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IContentRespository extension root for the Role entity.
    /// </summary>
    public interface IContentRepositoryRoleRepository
    {
        #region queries

        /// <summary>
        /// Finds a role by it's database id, returning null if the role could 
        /// not be found. If no role id is specified in the query then the anonymous 
        /// role is returned.
        /// </summary>
        /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
        IContentRepositoryRoleByIdQueryBuilder GetById(int? roleId);

        /// <summary>
        /// Finds a role with the specified role code, returning null if the role
        /// could not be found. Roles only have a RoleCode if they have been generated 
        /// from code rather than the GUI. For GUI generated roles use a 'get by id' 
        /// query.
        /// </summary>
        /// <param name="roleCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        IContentRepositoryRoleByCodeQueryBuilder GetByCode(string roleCode);

        #endregion
    }
}
