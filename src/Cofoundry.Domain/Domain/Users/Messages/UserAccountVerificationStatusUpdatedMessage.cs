namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user's account verification status has been updated,
    /// either being marked as verified, or had the verified flag removed.
    /// </summary>
    public class UserAccountVerificationStatusUpdatedMessage
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
        /// If <see langword="true"/> then the account has been updated to be verified; 
        /// otherwise if <see langword="false"/> then the account is no longer verified.
        /// </summary>
        public bool IsVerified { get; set; }
    }
}