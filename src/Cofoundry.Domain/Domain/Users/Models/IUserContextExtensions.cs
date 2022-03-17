namespace Cofoundry.Domain;

public static class IUserContextExtensions
{
    /// <summary>
    /// Indicates if the user is signed in i.e. <see cref="IUserContext.UserId"/>
    /// is not <see langword="null"/>.
    /// </summary>
    public static bool IsSignedIn(this IUserContext userContext)
    {
        return userContext.UserId.HasValue;
    }

    /// <summary>
    /// <see langword="true"/> if the user is in the Cofoundry <see cref="SuperAdminRole"/>.
    /// </summary>
    /// <param name="userContext"></param>
    /// <returns></returns>
    public static bool IsSuperAdmin(this IUserContext userContext)
    {
        return userContext.RoleCode == SuperAdminRole.Code && userContext.UserArea?.UserAreaCode == CofoundryAdminUserArea.Code;
    }

    /// <summary>
    /// Returns true if the user belongs to the Cofoundry user area.
    /// </summary>
    public static bool IsCofoundryUser(this IUserContext userContext)
    {
        return userContext.UserArea is CofoundryAdminUserArea;
    }
}
