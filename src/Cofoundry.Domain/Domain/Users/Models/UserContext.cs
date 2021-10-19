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
    }
}
