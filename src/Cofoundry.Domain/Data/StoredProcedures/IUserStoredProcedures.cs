using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// Data access abstraction over stored procedures for user entities and associated records.
    /// </summary>
    public interface IUserStoredProcedures
    {
        /// <summary>
        /// Adds an email domain name if it doesn't exist, returning the Id of
        /// the existing or newly created record.
        /// </summary>
        /// <param name="name">
        /// The name of the domain, which should be in a valid lowercased
        /// format e.g. "example.com" or "müller.example.com".
        /// Maps to <see cref="EmailDomain.Name"/></param>
        /// <param name="uniqueName">
        /// A unique name for the domain that is used for lookups. This is hashed 
        /// and mapped to <see cref="EmailDomain.NameHash"/>.
        /// </param>
        /// <param name="dateNow">
        /// The current date and time, which is used as the create date if
        /// a new entity is created.
        /// </param>
        /// <returns>The Id of the existing or newly created record.</returns>
        Task<int> AddEmailDomainIfNotExistsAsync(
            string name, 
            string uniqueName,
            DateTime dateNow
            );

        /// <summary>
        /// Writes a log entry to the FailedAuthenticationAttempt table indicating
        /// an unsuccessful login attempt occurred.
        /// </summary>
        /// <param name="userAreaCode">
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area 
        /// attempting to be authenticated.
        /// </param>
        /// <param name="username">
        /// The username used in the authentication attempt. This is expected to be in a 
        /// "uniquified" format, as this should have been already processed whenever 
        /// this needs to be called.
        /// </param>
        /// <param name="ipAddress">
        /// The string representation of the IP Address. This can be IP4, IP6
        /// or a hashed value less than 45 characters.
        /// </param>
        /// <param name="dateNow">
        /// The current date and time.
        /// </param>
        Task LogAuthenticationFailedAsync(
            string userAreaCode,
            string username,
            string ipAddress,
            DateTime dateNow
            );

        /// <summary>
        /// Writes a log entry to the UserAuthentication table indicating
        /// a successful authentication attempt.
        /// </summary>
        /// <param name="userId">
        /// The <see cref="User.UserId"/> of the user that successfully
        /// authenticated.
        /// </param>
        /// <param name="ipAddress">
        /// The string representation of the IP Address. This can be IP4, IP6
        /// or a hashed value less than 45 characters.
        /// </param>
        /// <param name="dateNow">The current date and time.</param>
        Task LogAuthenticationSuccessAsync(
            int userId,
            string ipAddress,
            DateTime dateNow
            );

        /// <summary>
        /// Determines if an authentication attempt is valid, returning <see langword="false"/> 
        /// if the attempt should be blocked due to too many failed attempts.
        /// </summary>
        /// <param name="userAreaCode">
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area 
        /// attempting to be authenticated.
        /// </param>
        /// <param name="username">
        /// The username used in the authentication attempt. This is expected to be in a 
        /// "uniquified" format, as this should have been already processed whenever 
        /// this needs to be called.
        /// </param>
        /// <param name="ipAddress">
        /// The string representation of the IP Address. This can be IP4, IP6
        /// or a hashed value less than 45 characters.
        /// </param>
        /// <param name="dateNow">The current date and time.</param>
        /// <param name="ipAddressRateLimitQuantity">
        /// The maximum number of failed authentication attempts allowed per IP address 
        /// during the rate limiting time window.
        /// </param>
        /// </param>
        /// <param name="ipAddressRateLimitWindowInSeconds">
        /// The time window to measure authentication attempts when rate limiting by IP 
        /// address.
        /// </param>
        /// <param name="usernameRateLimitQuantity">
        /// The maximum number of failed authentication attempts allowed per username 
        /// during the rate limiting time window.
        /// <param name="usernameRateLimitWindowInSeconds">
        /// The time window to measure authentication attempts when rate limiting by 
        /// username.
        /// </param>
        /// <returns><see langword="true"/> if the attmpet is valid and may proceed.</returns>
        Task<bool> IsAuthenticationAttemptValidAsync(
            string userAreaCode,
            string username,
            string ipAddress,
            DateTime dateNow,
            int? ipAddressRateLimitQuantity,
            int? ipAddressRateLimitWindowInSeconds,
            int? usernameRateLimitQuantity,
            int? usernameRateLimitWindowInSeconds
            );

        /// <summary>
        /// Soft-deletes a user account, removing personal data fields
        /// and any optional relations from the UnstructuredDataDependency
        /// table. The username is replaced with a <paramref name="pseudonym"/>
        /// which should be a unique non-identifying string of characters e.g. a GUID.
        /// </summary>
        /// <param name="userId">The <see cref="User.UserId"/> of the user to soft delete.</param>
        /// <param name="pseudonym">
        /// A unique string of up to 50 characters to replace the username with, typically
        /// this would be a GUID.
        /// </param>
        /// <param name="dateNow">The current date and time.</param>
        Task SoftDeleteAsync(
            int userId,
            string pseudonym,
            DateTime dateNow
            );

        /// <summary>
        /// General task for cleaning up stale user data. Currently this only removes data 
        /// from the <see cref="UserAuthenticationLog"/> and  <see cref="UserAuthenticationFailLog"/> tables.
        /// </summary>
        /// <param name="userAreaCode">
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area 
        /// to clean up records for.
        /// </param>
        /// <param name="authenticationLogRetentionPeriodInSeconds">
        /// The amount of time to keep records in the <see cref="UserAuthenticationLog"/> tables. 
        /// Defaults to the <see cref="DefaultRetentionPeriod"/>. If set to zero or less then data
        /// is kept indefinitely.
        /// </param>
        /// <param name="authenticationFailedLogRetentionPeriodInSeconds">
        /// The amount of time to keep records in the <see cref="UserAuthenticationFailLog"/> tables. 
        /// Defaults to the <see cref="DefaultRetentionPeriod"/>. If set to zero or less then data
        /// is kept indefinitely.
        /// </param>
        /// <param name="dateNow">The current date and time.</param>
        Task CleanupAsync(
            string userAreaCode,
            double authenticationLogRetentionPeriodInSeconds,
            double authenticationFailedLogRetentionPeriodInSeconds,
            DateTime dateNow
            );
    }
}