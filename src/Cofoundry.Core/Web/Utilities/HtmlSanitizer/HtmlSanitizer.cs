using Ganss.Xss;
using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;

namespace Cofoundry.Core.Web.Internal;

/// <summary>
/// An HtmlSanitizer that uses the Ganss.XSS.HtmlSanitizer
/// library for cleaning input. Unfortunately the ganss HtmlSanitizer 
/// shares the same class name, hopefully this doesn't cause too much 
/// confusion.
/// </summary>
public class HtmlSanitizer : IHtmlSanitizer
{
    private Ganss.Xss.HtmlSanitizer _defaultSanitizer;
    private readonly string _defaultBaseUrl = string.Empty;

    public HtmlSanitizer(
        IDefaultHtmlSanitizationRuleSetFactory defaultHtmlSanitizationRuleSetFactory
        )
    {
        var defaultHtmlSanitizationRuleSet = defaultHtmlSanitizationRuleSetFactory.Create();
        _defaultSanitizer = CreateSanitizer(defaultHtmlSanitizationRuleSet);
        _defaultBaseUrl = GetBaseUrl(defaultHtmlSanitizationRuleSet);
    }

    [return: NotNullIfNotNull(nameof(source))]
    public virtual string? Sanitize(IHtmlContent? source)
    {
        var stringContent = source?.ToString()?.Trim();
        if (stringContent == null) return null;

        IHtmlSanitizationRuleSet? ruleSet = null;
        if (source is ICustomSanitizationHtmlString)
        {
            ruleSet = ((ICustomSanitizationHtmlString)source).SanitizationRuleSet;
        }

        return Sanitize(stringContent, ruleSet);
    }

    [return: NotNullIfNotNull(nameof(source))]
    public virtual string? Sanitize(string? source, IHtmlSanitizationRuleSet? ruleSet = null)
    {
        if (source == null) return null;
        if (string.IsNullOrWhiteSpace(source)) return string.Empty;

        string result;

        if (ruleSet == null)
        {
            result = _defaultSanitizer.Sanitize(source, _defaultBaseUrl);
        }
        else
        {
            var sanitizer = CreateSanitizer(ruleSet);
            var baseUrl = GetBaseUrl(ruleSet);

            result = sanitizer.Sanitize(source, baseUrl);
        }

        return result;
    }

    /// <summary>
    /// Remove HTML tags from string
    /// </summary>
    [return: NotNullIfNotNull(nameof(source))]
    public virtual string? StripHtml(IHtmlContent? source)
    {
        if (source == null) return null;

        return StripHtml(source?.ToString());
    }

    /// <summary>
    /// Remove HTML tags from string
    /// </summary>
    /// <remakrs>
    /// See http://www.dotnetperls.com/remove-html-tags
    /// </remakrs>
    [return: NotNullIfNotNull(nameof(source))]
    public virtual string? StripHtml(string? source)
    {
        if (source == null) return null;

        char[] array = new char[source.Length];
        int arrayIndex = 0;
        bool inside = false;

        for (int i = 0; i < source.Length; i++)
        {
            char let = source[i];
            if (let == '<')
            {
                inside = true;
                continue;
            }
            if (let == '>')
            {
                inside = false;
                continue;
            }
            if (!inside)
            {
                array[arrayIndex] = let;
                arrayIndex++;
            }
        }

        return new string(array, 0, arrayIndex);
    }

    protected string GetBaseUrl(IHtmlSanitizationRuleSet ruleSet)
    {
        var ganssRuleSet = ruleSet as IGanssHtmlSanitizationRuleSet;
        if (ganssRuleSet == null) return string.Empty;

        return ganssRuleSet.BaseUrl;
    }

    protected Ganss.Xss.HtmlSanitizer CreateSanitizer(IHtmlSanitizationRuleSet ruleSet)
    {
        var options = new HtmlSanitizerOptions()
        {
            AllowedAtRules = ruleSet.PermittedAtRules,
            AllowedAttributes = ruleSet.PermittedAttributes,
            AllowedCssClasses = ruleSet.PermittedCssClasses,
            AllowedCssProperties = ruleSet.PermittedCssProperties,
            AllowedSchemes = ruleSet.PermittedSchemes,
            AllowedTags = ruleSet.PermittedTags,
            UriAttributes = ruleSet.PermittedUriAttributes
        };

        var sanitizer = new Ganss.Xss.HtmlSanitizer(options);
        var gnassRuleSet = ruleSet as IGanssHtmlSanitizationRuleSet;

        if (gnassRuleSet != null)
        {
            gnassRuleSet.Initialize(sanitizer);
        }

        return sanitizer;
    }
}
