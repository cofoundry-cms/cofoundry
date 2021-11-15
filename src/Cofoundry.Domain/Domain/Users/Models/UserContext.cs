using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
    /// <inheritdoc/>
    public class UserContext : IUserContext
    {
        public int? UserId { get; set; }

        public IUserAreaDefinition UserArea { get; set; }

        public bool IsPasswordChangeRequired { get; set; }

        public int? RoleId { get; set; }

        public string RoleCode { get; set; }

        public bool IsCofoundryUser()
        {
            return UserArea is CofoundryAdminUserArea;
        }

        /// <summary>
        /// An empty read-only <see cref="IUserContext"/> instance which can
        /// be used to represent a user that is not logged in.
        /// </summary>
        public static readonly IUserContext Empty = new EmptyUserContext();
    }
}
