using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a role by it's database id, returning a RoleDetails object if it 
    /// is found, otherwise null. If no role id is specified then the anonymous 
    /// role is returned.
    /// </summary>
    public class GetRoleDetailsByIdQuery : IQuery<RoleDetails>
    {
        public GetRoleDetailsByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with the specified role id.
        /// </summary>
        /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
        public GetRoleDetailsByIdQuery(int? roleId)
        {
            RoleId = roleId;
        }

        /// <summary>
        /// Database id of the role, or null to return the anonymous role.
        /// </summary>
        public int? RoleId { get; set; }
    }
}
