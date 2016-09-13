using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Allows the user table to partitioned into separate user areas e.g. Cofoundry Admin, Client. The 
    /// username for a user must be unique for each user area, but the same username can exist
    /// in different user areas which allows a person to be a member of each user area. User areas
    /// are very distinct partitions and shouldn't be used for something where Roles and Permissions 
    /// might be more appropriate (e.g. different levels of membership)
    /// </summary>
    public interface IUserArea
    {
        /// <summary>
        /// 3 letter code identifying this user area.
        /// </summary>
        string UserAreaCode { get; }

        /// <summary>
        /// Display name of the area, used in the Cofoundry admin panel
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates if users in this area can login using a password. If this
        /// is false the password field will be null and login will typically be via
        /// SSO or some other method.
        /// </summary>
        bool AllowPasswordLogin { get; }

        /// <summary>
        /// Indicates whether the user should login using thier email address
        /// as the username. Some SSO systems might provide only a username and not
        /// an email address so in this case the email address is allowed to be null. 
        /// </summary>
        bool UseEmailAsUsername { get; }
    }
}
