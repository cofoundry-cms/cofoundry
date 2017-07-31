using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// The super administrator role is always present in the system is
    /// is the highest level of access, includes all permissions and
    /// cannot be changed.
    /// </summary>
    public class SuperAdminRole : IRoleDefinition
    {
        /// <summary>
        /// Constant value for the Super Administrator role code
        /// </summary>
        public const string SuperAdminRoleCode = "SUP";

        public string Title { get { return "Super Administrator"; } }

        public string RoleCode { get { return SuperAdminRoleCode; } }

        public string UserAreaCode { get { return CofoundryAdminUserArea.AreaCode; } }
    }
}
