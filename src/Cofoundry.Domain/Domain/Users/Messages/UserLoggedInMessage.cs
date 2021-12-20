namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user successfully logs in.
    /// </summary>
    public class UserLoggedInMessage
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Id of the user that has logged in.
        /// </summary>
        public int UserId { get; set; }
    }
}
