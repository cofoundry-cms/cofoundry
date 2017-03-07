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
    public class UserDetails : UserSummary
    {
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
    }
}
