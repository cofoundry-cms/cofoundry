using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user successfully initiates the password 
    /// reset process via <see cref="InitiateUserPasswordResetRequestCommand"/>.
    /// </summary>
    public class UserPasswordResetInitiatedMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the user that having their password reset.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// A unique identifier required to authenticate when 
        /// resetting the password.
        /// </summary>
        public Guid UserPasswordResetRequestId { get; set; }
    }
}
