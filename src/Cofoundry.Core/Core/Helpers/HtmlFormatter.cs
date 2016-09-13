using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Globalization;

namespace Cofoundry.Core
{
    public static class HtmlFormatter
    {
        /// <summary>
        /// Converts line breaks from a textarea to html br tags
        /// </summary>
        public static string ConvertLineBreaksToBrTags(this string stIn)
        {
            return stIn
                   .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                   .Aggregate((a, b) => a + "<br/>" + b);
        }

        /// <summary>
        /// Converts urls in a string in to links.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="newWindow">if set to <c>true</c> [new window].</param>
        public static string ConvertUrlsToLinks(string text, BasicHtmlFormatOption formatOptions = BasicHtmlFormatOption.None)
        {
            var reg = new Regex(@"[""'=]?(http://|ftp://|https://|www\.|ftp\.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            
            var matchEval = new MatchEvaluator((match) =>
            {
                string href = match.Value;

                // if string starts within an HREF don't expand it
                if (href.StartsWith("=") || href.StartsWith("'") || href.StartsWith("\"")) return href;

                string displayText = href;

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

                string additionalAttributes = string.Empty;

                if (formatOptions.HasFlag(BasicHtmlFormatOption.LinksNoFollow))
                {
                    additionalAttributes += " rel='nofollow'";
                }

                if (formatOptions.HasFlag(BasicHtmlFormatOption.LinksToNewWindow))
                {
                    additionalAttributes += " target='_blank'";
                }

                return "<a href='" + href + "'" + additionalAttributes + ">" + displayText + "</a>";
            });

            return reg.Replace(text, matchEval);
        }

        /// <summary>
        /// Converts the specified plain text to have basic html formattign with line breaks and url links
        /// </summary>
        /// <param name="s">String to convert</param>
        /// <param name="formatOptions">Options to use in the converstion</param>
        /// <returns>HtmlString version of the input string formatted to basic html.</returns>
        public static IHtmlString ConvertToBasicHtml(string s, BasicHtmlFormatOption formatOptions = BasicHtmlFormatOption.None)
        {
            if (string.IsNullOrEmpty(s)) return new HtmlString(string.Empty);
            var html = ConvertLineBreaksToBrTags(s);
            html = ConvertUrlsToLinks(s, formatOptions);

            return new HtmlString(html);
        }

        /// <summary>
        /// Determins if the string contains html tags.
        /// </summary>
        /// <see cref="http://stackoverflow.com/a/204664"/>
        public static bool ContainsHtml(this string source)
        {
            var tagRegex = new Regex(@"<[^>]+>");
            return tagRegex.IsMatch(source);
        }
    }
}