using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A minimal role model with only essential identitifcation data.
    /// </summary>
    public class RoleMicroSummary
    {
        /// <summary>
        /// Database id of the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// The title is used to identify the role and select it in the admin UI
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A role must be assigned to a user area e.g. CofoundryAdminUserArea.
        /// </summary>
        public UserAreaMicroSummary UserArea { get; set; }
    }
}
