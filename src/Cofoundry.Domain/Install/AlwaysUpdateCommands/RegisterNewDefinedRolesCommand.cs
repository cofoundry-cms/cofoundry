using Cofoundry.Core.AutoUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Installation
{
    /// <summary>
    /// Runs the RegisterDefinedRolesCommand at startup, adding new roles
    /// defined in code to the system, but leaving existing ones alone.
    /// </summary>
    public class RegisterNewDefinedRolesCommand : IAlwaysRunUpdateCommand
    {
        public string Description
        {
            get
            {
                return "Register New Roles Defined in Code";
            }
        }
    }
}
