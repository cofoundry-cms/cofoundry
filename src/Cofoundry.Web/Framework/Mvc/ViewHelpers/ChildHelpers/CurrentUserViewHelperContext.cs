using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Collects together information about a user that can be
    /// used in a view.
    /// </summary>
    public class CurrentUserViewHelperContext
    {
        /// <summary>
        /// Indicates whether the user is logged in.
        /// </summary>
        public bool IsLoggedIn { get; set; }

        /// <summary>
        /// Information about the currently logged in user.
        /// </summary>
        public UserMicroSummary User { get; set; }

        /// <summary>
        /// Information about the role that the currently logged in user belongs.
        /// </summary>
        public RoleDetails Role { get; set; }
    }
}
