using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// User information relating to a login request
    /// </summary>
    public class UserLoginInfo
    {
        /// <summary>
        /// Database id of the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Code identifier of the user area this user belongs to. Each user 
        /// must be assigned to a user area (but not more than one).
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// True if a password change is required, this is set to true when an account is
        /// first created.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// True if the password hash version is out of date. If this true then the password 
        /// needs updating with the latest hash.
        /// </summary>
        public bool PasswordRehashNeeded { get; set; }
    }
}
