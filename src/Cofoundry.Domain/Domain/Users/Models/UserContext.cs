using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a users connection to the system at a specific point in time.
    /// </summary>
    public class UserContext : IUserContext
    {
        /// <summary>
        /// Id of the User if they are logged in.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// If the user is logged in this indicates which User Area they are logged into. Typically
        /// the only user area will be Cofoundry Admin, but some sites may have additional custom user areas
        /// e.g. a members area.
        /// </summary>
        public IUserAreaDefinition UserArea { get; set; }

        /// <summary>
        /// Indicates if the user should be required to change thier password when they log on.
        /// </summary>
        public bool IsPasswordChangeRequired { get; set; }

        /// <summary>
        /// The role that this user belongs to. If this is null then the anonymous role will be used.
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// Optional role code for the role this user belongs to. The role code indicates that the role
        /// is a code-first role.
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// Indicated if the user belongs to the CofoundryAdmin user area.
        /// </summary>
        public bool IsCofoundryUser()
        {
            return UserArea is CofoundryAdminUserArea;
        }
    }
}
