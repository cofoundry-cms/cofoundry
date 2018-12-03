using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    /// <summary>
    /// Defining a user area allows us to require users to
    /// sign-up or log in to access certain features of the site.
    /// 
    /// For more info see https://github.com/cofoundry-cms/cofoundry/wiki/User-Areas
    /// </summary>
    public class CustomerUserAreaDefinition : IUserAreaDefinition
    {
        /// <summary>
        /// Static access to the area code to make querying
        /// easier
        /// </summary>
        public const string Code = "CUS";

        /// <summary>
        /// Indicates if users in this area can login using a password. If this is false
        /// the password field will be null and login will typically be via SSO or some 
        /// other method.
        /// </summary>
        public bool AllowPasswordLogin => true;

        /// <summary>
        /// Display name of the area, used in the Cofoundry admin panel
        /// as the navigation link to manage your users. This should be singular
        /// because "Users" is appended to the link text.
        /// </summary>
        public string Name => "Customer";

        /// <summary>
        /// Indicates whether the user should login using thier email address as the username.
        /// Some SSO systems might provide only a username and not an email address so in
        /// this case the email address is allowed to be null.
        /// </summary>
        public bool UseEmailAsUsername => true;

        /// <summary>
        /// A unique 3 letter code identifying this user area. The cofoundry 
        /// user are uses the code "COF" so you can pick anything else!
        /// </summary>
        public string UserAreaCode => Code;

        public string LoginPath => "/customers/login/";

        /// <summary>
        /// Setting this to true means that this user area will be used as the default login
        /// schema which means the HttpContext.User property will be set to this identity.
        /// </summary>
        public bool IsDefaultAuthSchema => true;
    }
}
