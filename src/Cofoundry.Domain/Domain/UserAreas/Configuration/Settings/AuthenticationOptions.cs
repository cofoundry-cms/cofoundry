using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;
using Cofoundry.Core.ExecutionDurationRandomizer;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Options to control the behavior of the authentication process and related
    /// security features.
    /// </summary>
    public class AuthenticationOptions
    {
        public AuthenticationOptions()
        {
            MaxIPAttempts = 50;
            MaxUsernameAttempts = 20;
            MaxIPAttemptsBoundaryInMinutes = 60;
            MaxUsernameAttemptsBoundaryInMinutes = 60;
        }

        /// <summary>
        /// The maximum number of failed authentication attempts allowed per IP address 
        /// during the time window described by the MaxIPAttemptsBoundaryInMinutes
        /// property. The default value is 60 minutes.
        /// </summary>
        public int MaxIPAttempts { get; set; }

        /// <summary>
        /// The maximum number of failed authentication attempts allowed per username 
        /// during the time window described by the MaxUsernameAttemptsBoundaryInMinutes
        /// property. The default value is 40 minutes.
        /// </summary>
        public int MaxUsernameAttempts { get; set; }

        /// <summary>
        /// The time window to measure authentication attempts when testing for blocking by IP 
        /// address. The default value is 40 minutes.
        /// </summary>
        public int MaxIPAttemptsBoundaryInMinutes { get; set; }

        /// <summary>
        /// The time window to measure authentication attempts when testing for blocking by 
        /// username. The default value is 20 minutes.
        /// </summary>
        public int MaxUsernameAttemptsBoundaryInMinutes { get; set; }

        /// <summary>
        /// The randomized duration parameters when executing <see cref="AuthenticateUserCredentialsQuery"/> or
        /// any commands that authenticate credentials using this query. The lower bound of the duration should 
        /// exceed the expected execution time of the query to mitigate time-based enumeration attacks 
        /// to discover valid usernames. Defaults to a random duration between 1 and 1.5 seconds.
        /// </summary>
        public RandomizedExecutionDuration ExecutionDuration { get; set; } = new RandomizedExecutionDuration()
        {
            MinInMilliseconds = 1000,
            MaxInMilliseconds = 1500
        };

        /// <summary>
        /// The text to use to namespace the auth cookie. The user area
        /// code will be appended to this to make the cookiename, e.g.
        /// "MyAppAuth_COF". By default the cookie namespace is created
        /// using characters from the entry assembly name of your applicaiton.
        /// </summary>
        public string CookieNamespace { get; set; }

        /// <summary>
        /// Copies the options to a new instance, which can be modified
        /// without altering the base settings. This is used for user area
        /// specific configuration.
        /// </summary>
        public AuthenticationOptions Clone()
        {
            return new AuthenticationOptions()
            {
                MaxIPAttempts = MaxIPAttempts,
                MaxUsernameAttempts = MaxUsernameAttempts,
                MaxIPAttemptsBoundaryInMinutes = MaxIPAttemptsBoundaryInMinutes,
                MaxUsernameAttemptsBoundaryInMinutes = MaxUsernameAttemptsBoundaryInMinutes,
                ExecutionDuration = ExecutionDuration.Clone(),
                CookieNamespace = CookieNamespace
            };
        }
    }
}
