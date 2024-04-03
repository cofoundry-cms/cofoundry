using AngleSharp.Css.Dom;

namespace Cofoundry.Core.Web;

/// <summary>
/// A set of configuration rules for html sanitization used
/// by the IHtmlSanitizer.
/// </summary>
/// <inheritdoc/>
public class HtmlSanitizationRuleSet : IHtmlSanitizationRuleSet
{
    public IReadOnlySet<CssRuleType> PermittedAtRules { get; set; } = new HashSet<CssRuleType>();

    public IReadOnlySet<string> PermittedAttributes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> PermittedCssClasses { get; set; } = new HashSet<string>();

    public IReadOnlySet<string> PermittedCssProperties { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> PermittedSchemes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> PermittedTags { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> PermittedUriAttributes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
