using System.Collections.Immutable;
using Ganss.Xss;

namespace Cofoundry.Core.Web;

/// <summary>
/// Factory to create the default ruleset for the sanitizer. This uses the 
/// default settings from <see cref="HtmlSanitizerDefaults"/>, but with the addition
/// of the class attribute and the mailto scheme. See 
/// <see href="https://github.com/mganss/HtmlSanitizer"/> for more info.
/// </summary>
public class DefaultHtmlSanitizationRuleSetFactory : IDefaultHtmlSanitizationRuleSetFactory
{
    private readonly Lazy<HtmlSanitizationRuleSet> _defaultRulset = new(Initizalize);

    public IHtmlSanitizationRuleSet Create()
    {
        return _defaultRulset.Value;
    }

    private static HtmlSanitizationRuleSet Initizalize()
    {
        var ruleSet = new HtmlSanitizationRuleSet();

        ruleSet.PermittedAtRules = HtmlSanitizerDefaults
            .AllowedAtRules
            .ToImmutableHashSet();

        ruleSet.PermittedAttributes = HtmlSanitizerDefaults
            .AllowedAttributes
            .Append("class")
            .ToImmutableHashSet();

        ruleSet.PermittedCssClasses = HtmlSanitizerDefaults
            .AllowedClasses
            .ToImmutableHashSet();

        ruleSet.PermittedCssProperties = HtmlSanitizerDefaults
            .AllowedCssProperties
            .ToImmutableHashSet();

        ruleSet.PermittedSchemes = HtmlSanitizerDefaults
            .AllowedSchemes
            .Append("mailto")
            .ToImmutableHashSet();

        ruleSet.PermittedTags = HtmlSanitizerDefaults
            .AllowedTags
            .ToImmutableHashSet();

        ruleSet.PermittedUriAttributes = HtmlSanitizerDefaults
            .UriAttributes
            .ToImmutableHashSet();

        return ruleSet;
    }
}
