using AngleSharp.Css.Dom;

namespace Cofoundry.Core.Web;

/// <summary>
/// A set of configuration rules for html sanitization used
/// by the IHtmlSanitizer.
/// </summary>
/// <inheritdoc/>
public class HtmlSanitizationRuleSet : IHtmlSanitizationRuleSet
{
    public ISet<CssRuleType> PermittedAtRules { get; set; } = new HashSet<CssRuleType>();

    public ISet<string> PermittedAttributes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> PermittedCssClasses { get; set; } = new HashSet<string>();

    public ISet<string> PermittedCssProperties { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> PermittedSchemes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> PermittedTags { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> PermittedUriAttributes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
