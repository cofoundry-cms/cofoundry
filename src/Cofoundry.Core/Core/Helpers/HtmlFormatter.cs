﻿using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Cofoundry.Core;

public static class HtmlFormatter
{
    /// <summary>
    /// Converts line breaks from a textarea to html br tags
    /// </summary>
    [return: NotNullIfNotNull(nameof(stIn))]
    public static string? ConvertLineBreaksToBrTags(string? stIn)
    {
        if (stIn == null)
        {
            return null;
        }

        return stIn
               .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
               .Aggregate((a, b) => a + "<br/>" + b);
    }

    /// <summary>
    /// Converts urls in a string in to links.
    /// </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="formatOptions">
    /// Options used to describe how you want the links to be 
    /// formatted e.g. new window or no-follow.
    /// </param>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? ConvertUrlsToLinks(string? text, BasicHtmlFormatOption formatOptions = BasicHtmlFormatOption.None)
    {
        if (text == null)
        {
            return null;
        }

        var reg = new Regex(@"[""'=]?(http://|ftp://|https://|www\.|ftp\.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        var matchEval = new MatchEvaluator((match) =>
        {
            var href = match.Value;

            // if string starts within an HREF don't expand it
            if (href.StartsWith("=") || href.StartsWith("'") || href.StartsWith("\""))
            {
                return href;
            }

            var displayText = href;

            if (href.IndexOf("://") < 0)
            {
                if (href.StartsWith("www."))
                {
                    href = "http://" + href;
                }
                else if (href.StartsWith("ftp"))
                {
                    href = "ftp://" + href;
                }
                else if (href.IndexOf("@") > -1)
                {
                    href = "mailto:" + href;
                }
            }

            var additionalAttributes = string.Empty;

            if (formatOptions.HasFlag(BasicHtmlFormatOption.LinksNoFollow) && formatOptions.HasFlag(BasicHtmlFormatOption.LinksToNewWindow))
            {
                additionalAttributes += " target='_blank' rel='nofollow noopener'";
            }
            else if (formatOptions.HasFlag(BasicHtmlFormatOption.LinksNoFollow))
            {
                additionalAttributes += " rel='nofollow'";
            }
            else if (formatOptions.HasFlag(BasicHtmlFormatOption.LinksToNewWindow))
            {
                additionalAttributes += " target='_blank' rel='noopener'";
            }

            return "<a href='" + href + "'" + additionalAttributes + ">" + displayText + "</a>";
        });

        return reg.Replace(text, matchEval);
    }

    /// <summary>
    /// Converts the specified plain text to have basic html formattign with line breaks and url links
    /// </summary>
    /// <param name="s">String to convert</param>
    /// <param name="formatOptions">
    /// Options used to describe how you want the links to be 
    /// formatted e.g. new window or no-follow.
    /// </param>
    /// <returns>HtmlString version of the input string formatted to basic html.</returns>
    public static IHtmlContent ConvertToBasicHtml(string? s, BasicHtmlFormatOption formatOptions = BasicHtmlFormatOption.None)
    {
        if (string.IsNullOrEmpty(s))
        {
            return new HtmlString(string.Empty);
        }

        var html = ConvertLineBreaksToBrTags(s);
        html = ConvertUrlsToLinks(html, formatOptions);

        return new HtmlString(html);
    }

    /// <summary>
    /// Determines if the string contains html tags.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/204664.
    /// </remarks>
    public static bool ContainsHtml(string? source)
    {
        if (source == null)
        {
            return false;
        }

        var tagRegex = new Regex(@"<[^>]+>");
        return tagRegex.IsMatch(source);
    }
}
