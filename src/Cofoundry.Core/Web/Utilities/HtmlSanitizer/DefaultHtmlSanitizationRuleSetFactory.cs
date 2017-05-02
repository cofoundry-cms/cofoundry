using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Factory to create the default ruleset for the sanitizer
    /// </summary>
    public class DefaultHtmlSanitizationRuleSetFactory : IDefaultHtmlSanitizationRuleSetFactory
    {
        private Lazy<HtmlSanitizationRuleSet> _defaultRulset = new Lazy<HtmlSanitizationRuleSet>(() =>
        {
            // Original list courtesy of Robert Beal :
            // http://www.robertbeal.com/
            return new HtmlSanitizationRuleSet(new PermittedTag[] {
                    new PermittedTag("p", new string[]          {"style", "class", "align"}),
                    new PermittedTag("div", new string[]        {"style", "class", "align"}),
                    new PermittedTag("span", new string[]       {"style", "class"}),
                    new PermittedTag("br", new string[]         {"style", "class"}),
                    new PermittedTag("hr", new string[]         {"style", "class"}),
                    new PermittedTag("label", new string[]      {"style", "class"}),
                    new PermittedTag("h1", new string[]         {"style", "class"}),
                    new PermittedTag("h2", new string[]         {"style", "class"}),
                    new PermittedTag("h3", new string[]         {"style", "class"}),
                    new PermittedTag("h4", new string[]         {"style", "class"}),
                    new PermittedTag("h5", new string[]         {"style", "class"}),
                    new PermittedTag("h6", new string[]         {"style", "class"}),
                    new PermittedTag("font", new string[]       {"style", "class","color", "face", "size"}),
                    new PermittedTag("strong", new string[]     {"style", "class"}),
                    new PermittedTag("b", new string[]          {"style", "class"}),
                    new PermittedTag("em", new string[]         {"style", "class"}),
                    new PermittedTag("i", new string[]          {"style", "class"}),
                    new PermittedTag("u", new string[]          {"style", "class"}),
                    new PermittedTag("strike", new string[]     {"style", "class"}),
                    new PermittedTag("ol", new string[]         {"style", "class"}),
                    new PermittedTag("ul", new string[]         {"style", "class"}),
                    new PermittedTag("li", new string[]         {"style", "class"}),
                    new PermittedTag("blockquote", new string[] {"style", "class"}),
                    new PermittedTag("code", new string[]       {"style", "class"}),
                    new PermittedTag("pre", new string[]       {"style", "class"}),
                    new PermittedTag("a", new string[]          {"style", "class", "href", "title", "target", "rel"}),
                    new PermittedTag("img", new string[]        {"style", "class", "src", "height", "width", "alt", "title", "hspace", "vspace", "border"}),
                    new PermittedTag("table", new string[]      {"style", "class"}),
                    new PermittedTag("thead", new string[]      {"style", "class"}),
                    new PermittedTag("tbody", new string[]      {"style", "class"}),
                    new PermittedTag("tfoot", new string[]      {"style", "class"}),
                    new PermittedTag("th", new string[]         {"style", "class", "scope"}),
                    new PermittedTag("tr", new string[]         {"style", "class"}),
                    new PermittedTag("td", new string[]         {"style", "class", "colspan"}),
                    new PermittedTag("q", new string[]          {"style", "class", "cite"}),
                    new PermittedTag("cite", new string[]       {"style", "class"}),
                    new PermittedTag("sup", new string[]       {"style", "class"}),
                    new PermittedTag("sub", new string[]       {"style", "class"}),
                    new PermittedTag("abbr", new string[]       {"style", "class"}),
                    new PermittedTag("acronym", new string[]    {"style", "class"}),
                    new PermittedTag("del", new string[]        {"style", "class"}),
                    new PermittedTag("ins", new string[]        {"style", "class"})
                });
        });
        
        public HtmlSanitizationRuleSet Create()
        {
            return _defaultRulset.Value;
        }
    }
}
