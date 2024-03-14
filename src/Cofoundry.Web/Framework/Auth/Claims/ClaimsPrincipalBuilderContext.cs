namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="ClaimsPrincipalBuilderContext"/>.
/// </summary>
public class ClaimsPrincipalBuilderContext : IClaimsPrincipalBuilderContext
{
    /// <inheritdoc/>
    public int UserId { get; set; }

    /// <inheritdoc/>
    public string SecurityStamp { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string UserAreaCode { get; set; } = string.Empty;
}
