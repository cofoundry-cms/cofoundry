using System.Diagnostics.CodeAnalysis;
using Ganss.Xss;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Core.Web.Internal;

/// <summary>
/// An HtmlSanitizer that uses the Ganss.XSS.HtmlSanitizer
/// library for cleaning input. Unfortunately the ganss HtmlSanitizer 
/// shares the same class name, hopefully this doesn't cause too much 
/// confusion.
/// </summary>
public class HtmlSanitizer : IHtmlSanitizer
{
    private readonly Ganss.Xss.HtmlSanitizer _defaultSanitizer;
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
        if (stringContent == null)
        {
            return null;
        }

        IHtmlSanitizationRuleSet? ruleSet = null;
        if (source is ICustomSanitizationHtmlString customSanitizationHtmlString)
        {
            ruleSet = customSanitizationHtmlString.SanitizationRuleSet;
        }

        return Sanitize(stringContent, ruleSet);
    }

    [return: NotNullIfNotNull(nameof(source))]
    public virtual string? Sanitize(string? source, IHtmlSanitizationRuleSet? ruleSet = null)
    {
        if (source == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            return string.Empty;
        }

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
        if (source == null)
        {
            return null;
        }

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
        if (source == null)
        {
            return null;
        }

        var array = new char[source.Length];
        var arrayIndex = 0;
        var inside = false;

        for (var i = 0; i < source.Length; i++)
        {
            var let = source[i];
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
        if (ruleSet is not IGanssHtmlSanitizationRuleSet ganssRuleSet)
        {
            return string.Empty;
        }

        return ganssRuleSet.BaseUrl;
    }

    protected Ganss.Xss.HtmlSanitizer CreateSanitizer(IHtmlSanitizationRuleSet ruleSet)
    {
        var options = new HtmlSanitizerOptions()
        {
            AllowedAtRules = ruleSet.PermittedAtRules.ToHashSet(),
            AllowedAttributes = ruleSet.PermittedAttributes.ToHashSet(),
            AllowedCssClasses = ruleSet.PermittedCssClasses.ToHashSet(),
            AllowedCssProperties = ruleSet.PermittedCssProperties.ToHashSet(),
            AllowedSchemes = ruleSet.PermittedSchemes.ToHashSet(),
            AllowedTags = ruleSet.PermittedTags.ToHashSet(),
            UriAttributes = ruleSet.PermittedUriAttributes.ToHashSet()
        };

        var sanitizer = new Ganss.Xss.HtmlSanitizer(options);

        if (ruleSet is IGanssHtmlSanitizationRuleSet gnassRuleSet)
        {
            gnassRuleSet.Initialize(sanitizer);
        }

        return sanitizer;
    }
}
