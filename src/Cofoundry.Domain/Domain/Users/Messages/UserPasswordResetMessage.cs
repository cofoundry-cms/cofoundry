namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user password is reset via the admin
    /// panel or via <see cref="ResetUserPasswordCommand"/>, assigning a new
    /// temporary password. This is a separate action to the self-service 
    /// account recovery flow initiated using <see cref="InitiateUserAccountRecoveryByEmailCommand"/>.
    /// </summary>
    public class UserPasswordResetMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the user that has had their password reset.
        /// </summary>
        public int UserId { get; set; }
    }
}
