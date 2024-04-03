using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// The authentication status of a user at a specific point 
/// in time, typically representing the current context of a user during
/// the execution of a request. Users can log into multiple user 
/// areas concurrently, so an <see cref="IUserContext"/> is scoped
/// to a specific user area.
/// </para>
/// <para>
/// If the user is not logged in, then the properties of an <see cref="IUserContext"/> 
/// instance will either be <see langword="null"/> or default and should typically be 
/// represented by the <see cref="UserContext.Empty"/> instance.
/// </para>
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Id of the User if they are logged in; otherwise <see langword="null"/>.
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// If the user is logged in this indicates which User Area they are logged 
    /// into; otherwise this will be <see langword="null"/>. Typically the only 
    /// user area will be Cofoundry Admin, but some sites may have additional 
    /// custom user areas e.g. a members area.
    /// </summary>
    IUserAreaDefinition? UserArea { get; }

    /// <summary>
    /// Indicates if the user should be required to change thier password when they log on.
    /// </summary>
    bool IsPasswordChangeRequired { get; }

    /// <summary>
    /// Indicates if the account has been marked as verified. Verified accounts are an
    /// optional feature and this property can be ignored if you haven't implemented 
    /// account verification.
    /// </summary>
    bool IsAccountVerified { get; }

    /// <summary>
    /// The role that this user belongs to. If this is null then the anonymous role 
    /// should be used.
    /// </summary>
    int? RoleId { get; }

    /// <summary>
    /// If the user belongs to a code-first role, then this will be the string identifier
    /// for that role. Otherwise this will be <see langword="null"/>.
    /// </summary>
    string? RoleCode { get; }

    /// <summary>
    /// Indicates if the user is signed in i.e. <see cref="UserId"/>
    /// is not <see langword="null"/>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(UserId))]
    [MemberNotNullWhen(true, nameof(UserArea))]
    [MemberNotNullWhen(true, nameof(RoleId))]
    bool IsSignedIn()
    {
        return UserId.HasValue;
    }

    /// <summary>
    /// <see langword="true"/> if the user is in the Cofoundry <see cref="SuperAdminRole"/>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(UserId))]
    [MemberNotNullWhen(true, nameof(UserArea))]
    [MemberNotNullWhen(true, nameof(RoleId))]
    bool IsSuperAdmin()
    {
        return IsSignedIn() && RoleCode == SuperAdminRole.Code && UserArea?.UserAreaCode == CofoundryAdminUserArea.Code;
    }

    /// <summary>
    /// Returns true if the user belongs to the Cofoundry user area.
    /// </summary>
    [MemberNotNullWhen(true, nameof(UserId))]
    [MemberNotNullWhen(true, nameof(UserArea))]
    [MemberNotNullWhen(true, nameof(RoleId))]
    bool IsCofoundryUser()
    {
        return IsSignedIn() && UserArea is CofoundryAdminUserArea;
    }

    /// <summary>
    /// Maps the user context model to an <see cref="ISignedInUserContext"/> model
    /// if the user is signed in. If the user is not signed in then
    /// <see langword="null"/> is returned. The <see cref="ISignedInUserContext"/>
    /// is useful because it does not contain nullable properties for fields that
    /// are always present for signed in users.
    /// </summary>
    ISignedInUserContext? ToSignedInContext()
    {
        return SignedInUserContext.Map(this);
    }

    /// <summary>
    /// Maps the user context model for a signed in user to an <see cref="ISignedInUserContext"/> 
    /// model. If the user is not signed in then a <see cref="NotPermittedException"/> is 
    /// thrown. Note that this method is intended to be a convenience when you know a user is signed
    /// in and is not intended to replace <see cref="IPermissionValidationService"/>. The 
    /// <see cref="ISignedInUserContext"/> is useful because it does not contain 
    /// nullable properties for fields that are always present for signed in users.
    /// </summary>
    ISignedInUserContext ToRequiredSignedInContext()
    {
        var context = ToSignedInContext();

        if (context == null)
        {
            throw new NotPermittedException("User was expected to be signed in, but is not.");
        }

        return context;
    }
}
