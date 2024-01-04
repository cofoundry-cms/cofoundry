using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Default implementation of <see cref="IUserContext"/>.
/// </summary>
public class UserContext : IUserContext
{
    /// <inheritdoc/>
    public int? UserId { get; set; }

    /// <inheritdoc/>
    public IUserAreaDefinition? UserArea { get; set; }

    /// <inheritdoc/>
    public bool IsPasswordChangeRequired { get; set; }

    /// <inheritdoc/>
    public bool IsAccountVerified { get; set; }

    /// <inheritdoc/>
    public int? RoleId { get; set; }

    /// <inheritdoc/>
    public string? RoleCode { get; set; }

    /// <summary>
    /// An empty read-only <see cref="IUserContext"/> instance which can
    /// be used to represent a user that is not logged in.
    /// </summary>
    public static readonly IUserContext Empty = new EmptyUserContext();
}
