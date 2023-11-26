using AngleSharp.Css.Dom;

namespace Cofoundry.Core.Web;

/// <summary>
/// A set of configuration rules for html sanitization used
/// by the IHtmlSanitizer.
/// </summary>
/// <inheritdoc/>
public class HtmlSanitizationRuleSet : IHtmlSanitizationRuleSet
{
    public ISet<CssRuleType> PermittedAtRules { get; set; }

    public ISet<string> PermittedAttributes { get; set; }

    public ISet<string> PermittedCssClasses { get; set; }

    public ISet<string> PermittedCssProperties { get; set; }

    public ISet<string> PermittedSchemes { get; set; }

    public ISet<string> PermittedTags { get; set; }

    public ISet<string> PermittedUriAttributes { get; set; }
}
