namespace Cofoundry.Domain.Internal;

/// <summary>
/// An empty read-only <see cref="IUserContext"/> instance which can
/// be used to represent a user that is not logged in. This should be 
/// accessed via the <see cref="UserContext.Empty"/> static instance.
/// </summary>
/// <inheritdoc/>
public class EmptyUserContext : IUserContext
{
    public int? UserId { get; }

    public IUserAreaDefinition UserArea { get; }

    public bool IsPasswordChangeRequired { get; }

    public bool IsAccountVerified { get; }

    public int? RoleId { get; }

    public string RoleCode { get; }
}
