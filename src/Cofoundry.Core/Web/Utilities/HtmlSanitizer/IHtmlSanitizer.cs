using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Web;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// This is an HTML cleanup utility to remove potentially dangerous user input.
    /// </summary>
    public interface IHtmlSanitizer
    {
        string Sanitize(IHtmlString source);

        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="source">Html source</param>
        /// <param name="ruleSet">A custom set of tags to allow. first generic parameter is the tag, second is the allowed attributes.</param>
        /// <returns>Clean output</returns>
        string Sanitize(string source, HtmlSanitizationRuleSet ruleSet = null);

        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="source">Html source</param>
        /// <param name="ruleSet">A custom set of tags to allow. first generic parameter is the tag, second is the allowed attributes.</param>
        /// <returns>Clean output an an HtmlNode object</returns>
        HtmlNode SanitizeAsHtmlNode(string source, HtmlSanitizationRuleSet ruleSet = null);

        /// <summary>
        /// Takes a raw source and removes all HTML tags
        /// </summary>
        string StripHtml(string source);
    }
}
