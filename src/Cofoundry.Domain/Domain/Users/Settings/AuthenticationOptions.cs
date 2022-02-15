using Cofoundry.Core.ExecutionDurationRandomizer;
using Cofoundry.Core.Validation;
using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Options to control the behavior of the authentication process and related
    /// security features.
    /// </summary>
    public class AuthenticationOptions
    {
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
        /// The maximum number of failed authentication attempts to allow per IP address. The 
        /// default value is 50 attempts per hour.
        /// </summary>
        [ValidateObject]
        public RateLimitConfiguration IPAddressRateLimit { get; set; } = new RateLimitConfiguration(50, TimeSpan.FromHours(1));

        /// <summary>
        /// The maximum number of failed authentication attempts to allow per username. The 
        /// default value is 20 attempts per hour.
        /// </summary>
        [ValidateObject]
        public RateLimitConfiguration UsernameRateLimit { get; set; } = new RateLimitConfiguration(20, TimeSpan.FromHours(1));

        /// <summary>
        /// Copies the options to a new instance, which can be modified
        /// without altering the base settings. This is used for user area
        /// specific configuration.
        /// </summary>
        public AuthenticationOptions Clone()
        {
            return new AuthenticationOptions()
            {
                IPAddressRateLimit = IPAddressRateLimit.Clone(),
                UsernameRateLimit = IPAddressRateLimit.Clone(),
                ExecutionDuration = ExecutionDuration.Clone()
            };
        }
    }
}