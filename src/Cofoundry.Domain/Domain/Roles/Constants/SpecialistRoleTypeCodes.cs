using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class SpecialistRoleTypeCodes
    {
        /// <summary>
        /// The anonymous role is assigned to users not logged in.
        /// </summary>
        public static string Anonymous = "ANO";

        /// <summary>
        /// The super administrator role bypasses the permissions table and
        /// adds all permissions.
        /// </summary>
        public static string SuperAdministrator = "SUP";
    }
}
