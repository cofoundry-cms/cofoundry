using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Settings to do with authentication security features.
    /// </summary>
    public class AuthenticationSettings : CofoundryConfigurationSettingsBase
    {
        public AuthenticationSettings()
        {
            MaxIPAttempts = 50;
            MaxUsernameAttempts = 20;
            MaxIPAttemptsBoundaryInMinutes = 60;
            MaxUsernameAttemptsBoundaryInMinutes = 60;
            NumHoursPasswordResetLinkValid = 16;
        }

        /// <summary>
        /// The number of hours a password reset link is valid for. Defaults to 16 hours.
        /// </summary>
        public int NumHoursPasswordResetLinkValid { get; set; }

        /// <summary>
        /// The maximum number of failed login attempts allowed per IP address 
        /// during the time window described by the MaxIPAttemptsBoundaryInMinutes
        /// property. The default value is 60 minutes.
        /// </summary>
        public int MaxIPAttempts { get; set; }

        /// <summary>
        /// The maximum number of failed login attempts allowed per username 
        /// during the time window described by the MaxUsernameAttemptsBoundaryInMinutes
        /// property. The default value is 40 minutes.
        /// </summary>
        public int MaxUsernameAttempts { get; set; }

        /// <summary>
        /// The time window to measure login attempts when testing for blocking by IP 
        /// address. The default value is 40 minutes.
        /// </summary>
        public int MaxIPAttemptsBoundaryInMinutes { get; set; }

        /// <summary>
        /// The time window to measure login attempts when testing for blocking by 
        /// username. The default value is 20 minutes.
        /// </summary>
        public int MaxUsernameAttemptsBoundaryInMinutes { get; set; }

        /// <summary>
        /// The text to use to namespace the auth cookie. The user area
        /// code will be appended to this to make the cookiename, e.g.
        /// "MyAppAuth_COF". By default the cookie namespace is created
        /// using characters from the entry assembly name of your applicaiton.
        /// </summary>
        public string CookieNamespace { get; set; }
    }
}
