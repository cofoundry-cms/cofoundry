namespace Cofoundry.Web;

/// <inheritdoc/>
public class ClaimsPrincipalBuilderContext : IClaimsPrincipalBuilderContext
{
    public int UserId { get; set; }

    public string SecurityStamp { get; set; }

    public string UserAreaCode { get; set; }
}
