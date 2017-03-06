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
    /// Find a role with the specified specialist role type code, returning
    /// a RoleDetails object if one is found, otherwise null. Roles only
    /// have a SpecialistRoleTypeCode if they have been generated from code
    /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
    /// </summary>
    public class GetRoleDetailsBySpecialistRoleTypeCode : IQuery<RoleDetails>
    {
        public GetRoleDetailsBySpecialistRoleTypeCode()
        {
        }

        /// <summary>
        /// Initializes the query with the specified SpecialistRoleTypeCode
        /// </summary>
        /// <param name="specialistRoleTypeCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
        public GetRoleDetailsBySpecialistRoleTypeCode(string specialistRoleTypeCode)
        {
            SpecialistRoleTypeCode = specialistRoleTypeCode;
        }

        /// <summary>
        /// The code to find a matching role with. Roles only have a SpecialistRoleTypeCode 
        /// if they have been generated from code rather than the GUI. For GUI generated roles
        /// use GetRoleDetailsByIdQuery. Codes are 3 characters long (fixed length).
        /// </summary>
        public string SpecialistRoleTypeCode { get; set; }
    }
}
