using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A set of useful settings and options output as a js object for the front
    /// end UI. Includes properties from an IUserAreaDefinition but 
    /// could include other information too.
    /// </summary>
    public class UsersModuleOptions
    {
        /// <summary>
        /// 3 letter code identifying this user area.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Display name of the area, used in the Cofoundry admin panel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if users in this area can login using a password. If this
        /// is false the password field will be null and login will typically be via
        /// SSO or some other method.
        /// </summary>
        public bool AllowPasswordLogin { get; set; }

        /// <summary>
        /// Indicates whether the user should login using thier email address
        /// as the username. Some SSO systems might provide only a username and not
        /// an email address so in this case the email address is allowed to be null. 
        /// </summary>
        public bool UseEmailAsUsername { get; set; }
    }
}