using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Registers new roles defined in code via IRoleDefinition and initializes
    /// permissions when an IRoleInitializer has been implemented.
    /// </summary>
    public class RegisterPermissionsAndRolesCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// By default we don't update roles once they
        /// are in the system because we don't want to overwrite
        /// changes made in the UI, but you can force the update
        /// by running this command manually with this flag.
        /// </summary>
        public bool UpdateExistingRoles { get; set; }
    }
}
