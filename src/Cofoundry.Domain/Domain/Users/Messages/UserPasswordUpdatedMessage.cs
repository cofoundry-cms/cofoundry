namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user password is updated by
    /// a user. This message is not published when a password is
    /// reset by an admin user.
    /// </summary>
    public class UserPasswordUpdatedMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user was added to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the user that has been updated.
        /// </summary>
        public int UserId { get; set; }
    }
}
