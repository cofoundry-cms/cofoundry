namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user's activation status been updated,
    /// having either been deactivated or reactivated.
    /// </summary>
    public class UserActivationStatusUpdatedMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the user that has had their account verification status changed.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// If <see langword="true"/> then the account has been updated to be active; 
        /// otherwise if <see langword="false"/> then the account has been deactivated.
        /// </summary>
        public bool IsActive { get; set; }
    }
}