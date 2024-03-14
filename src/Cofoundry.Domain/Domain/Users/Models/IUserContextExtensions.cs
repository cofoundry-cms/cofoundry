using Cofoundry.Domain.Internal;

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
    /// Maps the user context model to an <see cref="ISignedInUserContext"/> model
    /// if the user is signed in. If the user is not signed in then
    /// <see langword="null"/> is returned. The <see cref="ISignedInUserContext"/>
    /// is useful because it does not contain nullable properties for fields that
    /// are always present for signed in users.
    /// </summary>
    public static ISignedInUserContext? ToSignedInContext(this IUserContext userContext)
    {
        return SignedInUserContext.Map(userContext);
    }

    /// <summary>
    /// Maps the user context model for a signed in user to an <see cref="ISignedInUserContext"/> 
    /// model. If the user is not signed in then a <see cref="NotPermittedException"/> is 
    /// thrown. Note that this method is intended to be a convenience when you know a user is signed
    /// in and is not intended to replace <see cref="IPermissionValidationService"/>. The 
    /// <see cref="ISignedInUserContext"/> is useful because it does not contain 
    /// nullable properties for fields that are always present for signed in users.
    /// </summary>
    public static ISignedInUserContext? ToRequiredSignedInContext(this IUserContext userContext)
    {
        var context = ToSignedInContext(userContext);

        if (context == null)
        {
            throw new NotPermittedException("User was expected to be signed in, but is not.");
        }

        return context;
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
