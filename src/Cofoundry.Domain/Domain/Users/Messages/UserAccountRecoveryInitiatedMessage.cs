using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user successfully initiates the account
    /// recovery process via <see cref="InitiateUserAccountRecoveryCommand"/>.
    /// </summary>
    public class UserAccountRecoveryInitiatedMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the user requesting to recover their account.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Unique identifier for the reset request, which is combined
        /// with an authorization code to create the unique <see cref="Token"/> that
        /// is used to identify and authorize the request via a url or similar mechanism.
        /// </summary>
        public Guid UserAccountRecoveryRequestId { get; set; }

        /// <summary>
        /// A token that can be used to identify and authenticates the password 
        /// reset request. Typically this is used in a url that is emailed to the
        /// user.
        /// </summary>
        public string Token { get; set; }
    }
}
