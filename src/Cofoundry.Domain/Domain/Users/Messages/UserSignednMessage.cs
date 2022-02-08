namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when a user successfully signs in.
    /// </summary>
    public class UserSignednMessage
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