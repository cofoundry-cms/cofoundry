namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published published when the email for a user is updated. The less
    /// specific <see cref="UserUpdatedMessage"/> is also published when an email is 
    /// updated, and if the email address is used as a username then a 
    /// <see cref="UserUsernameUpdatedMessage"/> is published too.
    /// </summary>
    public class UserEmailUpdatedMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the user that has been updated.
        /// </summary>
        public int UserId { get; set; }
    }
}
