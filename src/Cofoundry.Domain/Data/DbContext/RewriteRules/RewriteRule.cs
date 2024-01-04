namespace Cofoundry.Domain.Data;

/// <summary>
/// Rewrite rules can be used to redirect users from one url to another.
/// This functionality is incomplete and subject to change.
/// </summary>
/// <inheritdoc/>
public class RewriteRule : ICreateAuditable
{
    /// <summary>
    /// Identifier and database primary key.
    /// </summary>
    public int RewriteRuleId { get; set; }

    /// <summary>
    /// The incoming url to redirect from. Wildcard matching is supported
    /// by using an asterisk '*' at the end of the path.
    /// </summary>
    public string WriteFrom { get; set; } = string.Empty;

    /// <summary>
    /// The url to rewrite to.
    /// </summary>
    public string WriteTo { get; set; } = string.Empty;

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<RewriteRule>(nameof(Creator));
        set => _creator = value;
    }
}
