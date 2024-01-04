using System.Runtime.CompilerServices;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="ISignedInUserContext"/>.
/// </summary>
public class SignedInUserContext : ISignedInUserContext
{
    /// <inheritdoc/>
    public required int UserId { get; set; }

    /// <inheritdoc/>
    public required IUserAreaDefinition UserArea { get; set; }

    /// <inheritdoc/>
    public bool IsPasswordChangeRequired { get; set; }

    /// <inheritdoc/>
    public bool IsAccountVerified { get; set; }

    /// <inheritdoc/>
    public required int RoleId { get; set; }

    /// <inheritdoc/>
    public string? RoleCode { get; set; }

    public static ISignedInUserContext MapRequired(IUserContext userContext)
    {
        var mapped = Map(userContext);

        if (mapped == null)
        {
            throw new InvalidOperationException($"{nameof(userContext)} must be signed in to call {nameof(MapRequired)}");
        }

        return mapped;
    }

    public static ISignedInUserContext? Map(IUserContext userContext)
    {
        if (!userContext.IsSignedIn())
        {
            return null;
        }

        ThrowIfNull(userContext.RoleId, nameof(userContext.RoleId));
        ThrowIfNull(userContext.UserArea, nameof(userContext.RoleId));
        ThrowIfNull(userContext.UserId, nameof(userContext.RoleId));

        var mapped = new SignedInUserContext()
        {
            RoleId = userContext.RoleId.Value,
            UserArea = userContext.UserArea,
            UserId = userContext.UserId.Value,
            IsAccountVerified = userContext.IsAccountVerified,
            IsPasswordChangeRequired = userContext.IsPasswordChangeRequired,
            RoleCode = userContext.RoleCode
        };

        return mapped;

        void ThrowIfNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? propName = null)
        {
            if (value == null)
            {
                throw new InvalidOperationException($"{nameof(IUserContext)}.{propName} should not be null if a user is signed in");
            }
        }
    }
}
