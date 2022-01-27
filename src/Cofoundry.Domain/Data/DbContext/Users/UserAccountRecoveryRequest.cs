using System;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Users can initiate self-service account recovery (AKA "forgot password")  
    /// requests that are verified by sending a message with a unique link, typically 
    /// via email. This table tracks those requests and logs when they are completed.
    /// </summary>
    public class UserAccountRecoveryRequest
    {
        /// <summary>
        /// Primary key and unique identifier for the recovery request, which is combined
        /// with the <see cref="AuthorizationCode"/> to create the unique token that
        /// is used to identify and authorize the request via a url or similar mechanism.
        /// </summary>
        public Guid UserAccountRecoveryRequestId { get; set; }

        /// <summary>
        /// The id of the user requesting the account recovery.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The the user requesting the account recovery.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// A cryptographically strong random code that is used to authenticate the 
        /// request before resetting the password.
        /// </summary>
        public string AuthorizationCode { get; set; }

        /// <summary>
        /// The IPAddress of the client that initiated the request.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// The date the request was initiated. This is used to calculate the
        /// expiry date, based on the configured expiry window.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The date at which the request was marked invalid e.g. when a password is
        /// changed, any pending reset requests are invalidated.
        /// </summary>
        public DateTime? InvalidatedDate { get; set; }

        /// <summary>
        /// The date at which the request was completed and the password changed. 
        /// This will be <see langword="null"/> if the request has not been completed.
        /// </summary>
        public DateTime? CompletedDate { get; set; }
    }
}