using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// A set of configuration rules for html sanitization used
    /// by the IHtmlSanitizer.
    /// </summary>
    public class HtmlSanitizationRuleSet : IHtmlSanitizationRuleSet
    {
        /// <summary>
        /// Collection html tags to permit e.g. "a" and "div".
        /// </summary>
        public ISet<string> PermittedTags { get; set; }

        /// <summary>
        /// Collection of html tag permit to allow e.g. "title" and "alt".
        /// </summary>
        public ISet<string> PermittedAttributes { get; set; }

        /// <summary>
        /// Collection of http schemas to permit e.g. "http", "https" and "mailto".
        /// </summary>
        public ISet<string> PermittedSchemes { get; set; }

        /// <summary>
        /// Collection html tags that are permitted to have uri properties e.g. "src", "href".
        /// </summary>
        public ISet<string> PermittedUriAttributes { get; set; }

        /// <summary>
        /// Collection of style properties to permit e.g. "font" and "margin".
        /// </summary>
        public ISet<string> PermittedCssProperties { get; set; }
    }
}
