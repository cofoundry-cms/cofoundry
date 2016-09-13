using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Text;
using System.Web;
using System.Net;
using System.Web.Security.AntiXss;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// This is an HTML cleanup utility combining the benefits of the
    /// HtmlAgilityPack to parse raw HTML and the AntiXss library
    /// to remove potentially dangerous user input.
    ///
    /// Additionally it uses a list created by Robert Beal to limit
    /// the number of allowed tags and attributes to a sensible level
    /// </summary>
    /// <remarks>
    /// Taken from:
    /// http://eksith.wordpress.com/2012/02/13/antixss-4-2-breaks-everything/
    /// </remarks>
    public class HtmlSanitizer : IHtmlSanitizer
    {
        #region constructor

        private readonly IDefaultHtmlSanitizationRuleSetFactory _defaultHtmlSanitizationRuleSetFactory;

        public HtmlSanitizer(
            IDefaultHtmlSanitizationRuleSetFactory defaultHtmlSanitizationRuleSetFactory
            )
        {
            _defaultHtmlSanitizationRuleSetFactory = defaultHtmlSanitizationRuleSetFactory;
        }

        #endregion

        public string Sanitize(IHtmlString source)
        {
            if (source == null) return string.Empty;
            HtmlSanitizationRuleSet ruleSet = null;
            if (source is CustomSanitizationHtmlString)
            {
                ruleSet = ((CustomSanitizationHtmlString)source).SanitizationRuleSet;
            }

            return Sanitize(source.ToString().Trim(), ruleSet);
        }
        
        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="source">Html source</param>
        /// <param name="ruleSet">A custom set of tags to allow. first generic parameter is the tag, second is the allowed attributes.</param>
        /// <returns>Clean output</returns>
        public string Sanitize(string source, HtmlSanitizationRuleSet ruleSet = null)
        {
            if (source == null) return null;

            var node = SanitizeAsHtmlNode(source, ruleSet);
            if (node == null) return string.Empty;

            return node.InnerHtml;
        }

        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="source">Html source</param>
        /// <param name="ruleSet">A custom set of tags to allow. first generic parameter is the tag, second is the allowed attributes.</param>
        /// <returns>Clean output an an HtmlNode object</returns>
        public HtmlNode SanitizeAsHtmlNode(string source, HtmlSanitizationRuleSet ruleSet = null)
        {
            if (source == null) return null;
            if (source.Trim() == string.Empty) return null;

            source = FixUnEncodedLt(source);
            HtmlDocument html = GetHtml(source);
            if (html == null) return null;

            // All the nodes
            HtmlNode allNodes = html.DocumentNode;

            // Select whitelist tag names
            ruleSet = ruleSet ?? _defaultHtmlSanitizationRuleSetFactory.Create();

            // Scrub tags not in whitelist
            CleanNodes(allNodes, ruleSet.PermittedTags);

            // Filter the attributes of the remaining
            foreach (var tag in ruleSet.PermittedTags)
            {
                IEnumerable<HtmlNode> nodes = (from n in allNodes.DescendantsAndSelf()
                                               where n.Name == tag.Tag
                                               select n);
                // No nodes? Skip.
                if (nodes == null) continue;

                foreach (var n in nodes)
                {
                    // No attributes? Skip.
                    if (!n.HasAttributes) continue;

                    // Get all the allowed attributes for this tag
                    HtmlAttribute[] attr = n.Attributes.ToArray();
                    foreach (HtmlAttribute a in attr)
                    {
                        if (!tag.PermittedAttributes.Contains(a.Name))
                        {
                            a.Remove(); // Attribute wasn't in the whitelist
                        }
                        else
                        {
                            // *** New workaround. This wasn't necessary with the old library
                            if (a.Name == "href" || a.Name == "src")
                            {
                                a.Value = (!string.IsNullOrEmpty(a.Value)) ? a.Value.Replace("\r", "").Replace("\n", "") : "";
                                a.Value =
                                    (!string.IsNullOrEmpty(a.Value) &&
                                    (a.Value.IndexOf("javascript") < 10 || a.Value.IndexOf("eval") < 10)) ?
                                    a.Value.Replace("javascript", "").Replace("eval", "") : a.Value;
                            }
                            else if (a.Name == "class" || a.Name == "style")
                            {
                                a.Value = AntiXssEncoder.CssEncode(a.Value);
                            }
                            else
                            {
                                a.Value = HttpUtility.HtmlAttributeEncode(HttpUtility.HtmlDecode(a.Value)); // amended to prevent double encoding
                            }
                        }
                    }
                }
            }

            if (ruleSet.OnHtmlSanitized != null) ruleSet.OnHtmlSanitized(html);

            // *** New workaround (DO NOTHING HAHAHA! Fingers crossed)
            return allNodes;

            // *** Original code below

            /*
            // Anything we missed will get stripped out
            return
                Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(allNodes.InnerHtml);
                */
        }

        /// <summary>
        /// Remove HTML tags from string
        /// </summary>
        /// <see cref="http://www.dotnetperls.com/remove-html-tags"/>
        public string StripHtml(string source)
        {
            if (source == null) return string.Empty;

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
        
        #region private members

        /// <summary>
        /// HtmlAgilityPack has issues with unencoded '&lt;' characters, so this preprocesses
        /// the string encoding them properly.
        /// </summary>
        /// <remarks>
        /// see http://stackoverflow.com/a/5627272/716689
        /// </remarks>
        static string FixUnEncodedLt(string htmlInput)
        {
            // Stores the index of the last unclosed '<' character, or -1 if the last '<' character is closed.
            int lastGt = -1;

            // This list will be populated with all the unclosed '<' characters.
            List<int> gtPositions = new List<int>();

            // Collect the unclosed '<' characters.
            for (int i = 0; i < htmlInput.Length; i++)
            {
                // JM: Slight modification to attempt to account for < and > signs in the same sentence, assumes '< ' is not part of an html tag.
                if (htmlInput.Length > i + 1 
                    && htmlInput[i] == '<' 
                    && htmlInput[i + 1] == ' ')
                {
                    gtPositions.Add(i);
                }
                else if (htmlInput[i] == '<')
                {
                    if (lastGt != -1)
                    {
                        gtPositions.Add(lastGt);
                    }

                    lastGt = i;
                }
                else if (htmlInput[i] == '>')
                {
                    lastGt = -1;
                }
            }

            if (lastGt != -1)
                gtPositions.Add(lastGt);

            // If no unclosed '<' characters are found, then just return the input string.
            if (gtPositions.Count == 0)
                return htmlInput;

            // Build the output string, replace all unclosed '<' character by "&lt;".
            StringBuilder htmlOutput = new StringBuilder(htmlInput.Length + 3 * gtPositions.Count);
            int start = 0;

            foreach (int gtPosition in gtPositions)
            {
                htmlOutput.Append(htmlInput.Substring(start, gtPosition - start));
                htmlOutput.Append("&lt;");
                start = gtPosition + 1;
            }

            htmlOutput.Append(htmlInput.Substring(start));
            return htmlOutput.ToString();
        }

        /// <summary>
        /// Recursively delete nodes not in the whitelist
        /// </summary>
        private static void CleanNodes(HtmlNode node, IEnumerable<PermittedTag> whitelist)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                var tag = whitelist.FirstOrDefault(t => t.Tag == node.Name);
                if (tag == null)
                {
                    node.ParentNode.RemoveChild(node, true);
                    return; // We're done
                }
                if (tag.TagAction != null) tag.TagAction(node);
            }

            if (node.HasChildNodes) CleanChildren(node, whitelist);
        }

        /// <summary>
        /// Apply CleanNodes to each of the child nodes
        /// </summary>
        private static void CleanChildren(HtmlNode parent, IEnumerable<PermittedTag> whitelist)
        {
            for (int i = parent.ChildNodes.Count - 1; i >= 0; i--)
                CleanNodes(parent.ChildNodes[i], whitelist);
        }

        /// <summary>
        /// Helper function that returns an HTML document from text
        /// </summary>
        private static HtmlDocument GetHtml(string source)
        {
            HtmlDocument html = new HtmlDocument();
            html.OptionFixNestedTags = true;
            html.OptionAutoCloseOnEnd = true;
            html.OptionDefaultStreamEncoding = Encoding.UTF8;

            html.LoadHtml(source);

            // Encode any code blocks independently so they won't
            // be stripped out completely when we do a final cleanup
            foreach (var n in html.DocumentNode.DescendantsAndSelf())
            {
                if (n.Name == "code")
                {
                    //** Code tag attribute vulnerability fix 28-9-12 (thanks to Natd)
                    HtmlAttribute[] attr = n.Attributes.ToArray();
                    foreach (HtmlAttribute a in attr)
                    {
                        if (a.Name != "style" && a.Name != "class") { a.Remove(); }
                    } //** End fix
                    n.InnerHtml = WebUtility.HtmlEncode(n.InnerHtml);
                }
            }

            return html;
        }

        #endregion
    }
}
