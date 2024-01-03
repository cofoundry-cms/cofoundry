using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;

namespace Cofoundry.Core.Web;

/// <summary>
/// This is an HTML cleanup utility to remove potentially dangerous user input.
/// </summary>
public interface IHtmlSanitizer
{
    /// <summary>
    /// Takes raw HTML input and sanitizes it using the default
    /// ruleset provided by <see cref="IDefaultHtmlSanitizationRuleSetFactory"/>.
    /// </summary>
    /// <param name="source">
    /// HTml content to sanitize. If <see langword="null"/> then the result will also be <see langword="null"/>.
    /// </param>
    [return: NotNullIfNotNull(nameof(source))]
    string? Sanitize(IHtmlContent? source);

    /// <summary>
    /// Takes raw HTML input and sanitizes it using the specified 
    /// <paramref name="ruleSet"/>, falling back to the default ruleset 
    /// provided by <see cref="IDefaultHtmlSanitizationRuleSetFactory"/>
    /// if <paramref name="ruleSet"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="source">
    /// String to sanitize. If <see langword="null"/> then the result will also be <see langword="null"/>.
    /// </param>
    /// <param name="ruleSet">
    /// A custom ruleset referencing the peritted tags and other rules.
    /// </param>
    [return: NotNullIfNotNull(nameof(source))]
    string? Sanitize(string? source, IHtmlSanitizationRuleSet? ruleSet = null);

    /// <summary>
    /// Takes a raw string and removes all HTML tags. If the input if 
    /// <see langword="null"/> then the result will also be <see langword="null"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(source))]
    string? StripHtml(string? source);

    /// <summary>
    /// Takes a raw <see cref="HtmlString"/> and removes all HTML tags. If the input if 
    /// <see langword="null"/> then the result will also be <see langword="null"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(source))]
    string? StripHtml(IHtmlContent? source);
}
