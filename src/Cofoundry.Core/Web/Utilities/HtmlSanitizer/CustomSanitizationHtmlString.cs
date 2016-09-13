using System.Web;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Represents an IHtmlString with a custom sanitization ruleset, useful if the default ruleset in the HtmlSanitizer is too restricitve, or not 
    /// restrictive enough.
    /// </summary>
    public class CustomSanitizationHtmlString : HtmlString
    {
        public CustomSanitizationHtmlString(string value)
            : base(value)
        {
        }

        public HtmlSanitizationRuleSet SanitizationRuleSet { get; protected set; }
    }
}
