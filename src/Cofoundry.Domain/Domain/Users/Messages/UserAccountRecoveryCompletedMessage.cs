using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user successfully completes the account 
    /// recovery process via <see cref="CompleteUserAccountRecoveryCommand"/>. This
    /// command will also trigger <see cref="UserPasswordUpdatedMessage"/>.
    /// </summary>
    public class UserAccountRecoveryCompletedMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the user recovering their account.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Primary key and unique identifier for the recovery request, which is combined
        /// with an authorization code to create the unique token that
        /// is used to identify and authorize the request via a url or similar mechanism.
        /// </summary>
        public Guid UserAccountRecoveryRequestId { get; set; }
    }
}
