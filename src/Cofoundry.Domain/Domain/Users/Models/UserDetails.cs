using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Full representation of a user, containing all properties. Users 
    /// are partitioned by user area so a user might be a Cofoundry admin 
    /// user or could belong to a custom user area. Users cannot belong to 
    /// more than one user area.
    /// </summary>
    public class UserDetails : UserMicroSummary
    {
        /// <summary>
        /// Each user must be assigned to a role which provides
        /// information about the actions a user is permitted to 
        /// perform.
        /// </summary>
        public RoleDetails Role { get; set; }

        /// <summary>
        /// The date the user last logged into the application. May be
        /// null if the user has not logged in yet.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// The date the password was last changed or the that the password
        /// was first set (account create date)
        /// </summary>
        public DateTime LastPasswordChangeDate { get; set; }

        /// <summary>
        /// True if a password change is required, this is set to true when an account is
        /// first created.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// Data detailing who created the user and when.
        /// </summary>
        public CreateAuditData AuditData { get; set; }
    }
}
