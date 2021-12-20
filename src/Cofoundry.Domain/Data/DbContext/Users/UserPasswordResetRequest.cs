using System;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Users can initiate self-service password reset requests that
    /// are verified by sending a message with a unique link, typically 
    /// via email. This table tracks those requests and logs when they are 
    /// completed.
    /// </summary>
    public partial class UserPasswordResetRequest
    {
        /// <summary>
        /// A unique identifier required to authenticate when 
        /// resetting the password.
        /// </summary>
        public Guid UserPasswordResetRequestId { get; set; }

        /// <summary>
        /// The id of the user requesting the password reset.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The the user requesting the password reset.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// A token used to authenticate when resetting the password.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The IPAddress of the client that initiated the request.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// The date the request was initiated.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// <see langword="true"/> if the request has been completed and
        /// the password has been changed; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsComplete { get; set; }
    }
}
