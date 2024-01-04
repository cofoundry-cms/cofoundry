using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public static class UserAccountStatusMapper
{
    public static UserAccountStatus Map(User dbUser)
    {
        if (dbUser.DeletedDate.HasValue)
        {
            return UserAccountStatus.Deleted;
        }
        else if (dbUser.DeactivatedDate.HasValue)
        {
            return UserAccountStatus.Deactivated;
        }

        return UserAccountStatus.Active;
    }
}
