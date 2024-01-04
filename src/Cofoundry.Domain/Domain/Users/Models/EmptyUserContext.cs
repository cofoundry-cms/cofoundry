namespace Cofoundry.Domain.Internal;

/// <summary>
/// An empty read-only <see cref="IUserContext"/> instance which can
/// be used to represent a user that is not logged in. This should be 
/// accessed via the <see cref="UserContext.Empty"/> static instance.
/// </summary>
public class EmptyUserContext : IUserContext
{
    /// <inheritdoc/>
    public int? UserId { get; }

    /// <inheritdoc/>
    public IUserAreaDefinition? UserArea { get; }

    /// <inheritdoc/>
    public bool IsPasswordChangeRequired { get; }

    /// <inheritdoc/>
    public bool IsAccountVerified { get; }

    /// <inheritdoc/>
    public int? RoleId { get; }

    /// <inheritdoc/>
    public string? RoleCode { get; }
}
