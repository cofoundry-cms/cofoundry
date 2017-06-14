using Microsoft.AspNetCore.Html;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Represents an IHtmlString with a custom sanitization ruleset, useful if the default ruleset in the 
    /// HtmlSanitizer is too restricitve, or not restrictive enough.
    /// </summary>
    public interface ICustomSanitizationHtmlString : IHtmlContent
    {
        /// <summary>
        /// The rule set to use when sanitizing this instance.
        /// </summary>
        IHtmlSanitizationRuleSet SanitizationRuleSet { get; }
    }
}
