namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a user is added.
    /// </summary>
    public class UserAddedMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user was added to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the newly added user.
        /// </summary>
        public int UserId { get; set; }
    }
}
