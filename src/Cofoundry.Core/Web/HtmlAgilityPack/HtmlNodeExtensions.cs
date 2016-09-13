using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Core.Web
{
    public static class HtmlNodeExtensions
    {
        /// <summary>
        /// Similar to SelectNodes in that it quries an html node with an xPath query
        /// except it returns Enumerable.Empty if no nodes are found ratehr than null.
        /// </summary>
        /// <param name="xPathQuery">xPath query to run</param>
        public static IEnumerable<HtmlNode> QueryNodes(this HtmlNode node, string xPathQuery)
        {
            var childNodes = node.SelectNodes(xPathQuery);

            if (childNodes != null)
            {
                foreach (var childNode in childNodes)
                {
                    yield return childNode;
                }
            }
        }

        /// <summary>
        /// Gets all the text content within the queried nodes and returns, trimming the results.
        /// e.g. use this to get all the plain text within multiple p elements.
        /// </summary>
        /// <param name="xPathQuery">xPath query to run</param>
        public static IEnumerable<string> GetNonEmptyInnerText(this HtmlNode node, string xPathQuery)
        {
            return node
                .QueryNodes(xPathQuery)
                .Where(n => !string.IsNullOrWhiteSpace(n.InnerText))
                .Select(n => n.InnerText.Trim());
        }

        /// <summary>
        /// Merges existing attributes on a node with the specified attribute collection. The
        /// attributes are 
        /// </summary>
        /// <param name="dataAttributes">Attributes to merge</param>
        /// <returns>The HtmlNode object for method chaining</returns>
        public static HtmlNode MergeAttributes(this HtmlNode node, IDictionary<string, string> dataAttributes)
        {
            if (dataAttributes == null) return node;

            foreach (var attr in dataAttributes)
            {
                if (attr.Key.Equals("class", StringComparison.OrdinalIgnoreCase))
                {
                    MergeCss(node, attr.Value);
                }
                else
                {
                    node.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            return node;
        }

        /// <summary>
        /// Merges the specified css classes with those already on the HtmlNode element.
        /// </summary>
        /// <param name="css">Space delimited list of css classes to merge.</param>
        /// <returns>The HtmlNode object for method chaining</returns>
        public static HtmlNode MergeCss(this HtmlNode node, string css)
        {
            var cssDelimiter = new char[] { ' ' };

            var newClasses = (css ?? string.Empty).Split(cssDelimiter, StringSplitOptions.RemoveEmptyEntries);
            var existingClasses = node.GetAttributeValue("class", string.Empty).Split(cssDelimiter, StringSplitOptions.RemoveEmptyEntries);

            var allStyles = String.Join(" ", existingClasses.Union(newClasses).Distinct());
            node.SetAttributeValue("class", allStyles);

            return node;
        }
    }
}
