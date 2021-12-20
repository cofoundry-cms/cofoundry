namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when the email for a user is updated.
    /// </summary>
    public class UserEmailUpdatedMessage
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
