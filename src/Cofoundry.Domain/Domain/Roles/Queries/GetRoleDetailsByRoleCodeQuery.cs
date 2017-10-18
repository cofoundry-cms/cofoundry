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
    /// Find a role with the specified role code, returning
    /// a RoleDetails object if one is found, otherwise null. Roles only
    /// have a RoleCode if they have been generated from code
    /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
    /// </summary>
    public class GetRoleDetailsByRoleCodeQuery : IQuery<RoleDetails>
    {
        public GetRoleDetailsByRoleCodeQuery()
        {
        }

        /// <summary>
        /// Initializes the query with the specified RoleCode
        /// </summary>
        /// <param name="roleCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        public GetRoleDetailsByRoleCodeQuery(string roleCode)
        {
            RoleCode = roleCode;
        }

        /// <summary>
        /// The code to find a matching role with. Roles only have a RoleCode 
        /// if they have been generated from code rather than the GUI. For GUI generated roles
        /// use GetRoleDetailsByIdQuery. Codes are 3 characters long (fixed length).
        /// </summary>
        public string RoleCode { get; set; }
    }
}
