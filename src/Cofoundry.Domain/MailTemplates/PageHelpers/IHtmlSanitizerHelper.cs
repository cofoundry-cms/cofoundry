﻿using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain;

/// <summary>
/// Helper for sanitizing html before it output to the page. You'd typically
/// want to use this when rendering out user inputted data which may be 
/// vulnerable to XSS attacks.
/// </summary>
public interface IHtmlSanitizerHelper
{
    /// <summary>
    /// Sanitizes the specified string using the default IHtmlSanitizer
    /// to remove potentially dangerous markup.
    /// </summary>
    /// <param name="s">String content to sanitize</param>
    IHtmlContent Sanitize(string? s);

    /// <summary>
    /// Sanitizes the specified string using the default IHtmlSanitizer
    /// to remove potentially dangerous markup.
    /// </summary>
    /// <param name="s">Html content to sanitize</param>
    IHtmlContent Sanitize(IHtmlContent? s);

    /// <summary>
    /// Takes a string and removes all HTML tags
    /// </summary>
    /// <param name="source">String content to sanitize</param>
    [return: NotNullIfNotNull(nameof(source))]
    string? StripHtml(string? source);

    /// <summary>
    /// Takes a raw source and removes all HTML tags
    /// </summary>
    /// <param name="source">HtmlString content to sanitize</param>
    [return: NotNullIfNotNull(nameof(source))]
    string? StripHtml(HtmlString? source);
}
