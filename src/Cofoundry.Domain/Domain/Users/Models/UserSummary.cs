using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// The UserSummary is a reduced representation of a user. Building on 
    /// the UserMicroSummary, the Usersummary contains additional audit 
    /// and role data. Users are partitioned by user area so a user might be 
    /// a Cofoundry admin user or could belong to a custom user area. Users 
    /// cannot belong to more than one user area.
    /// </summary>
    public class UserSummary : UserMicroSummary, ICreateAudited
    {
        /// <summary>
        /// Each user must be assigned to a role which provides
        /// information about the actions a user is permitted to 
        /// perform.
        /// </summary>
        public RoleMicroSummary Role { get; set; }

        /// <summary>
        /// The date the user last logged into the application. May be
        /// null if the user has not logged in yet.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Data detailing who created the user and when.
        /// </summary>
        public CreateAuditData AuditData { get; set; }
    }
}
