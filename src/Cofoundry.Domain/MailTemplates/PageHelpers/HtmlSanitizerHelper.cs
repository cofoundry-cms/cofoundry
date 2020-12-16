using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// An HTML cleanup utility to remove potentially dangerous user input.
    /// </summary>
    public class HtmlSanitizerHelper : IHtmlSanitizerHelper
    {
        private readonly IHtmlSanitizer _htmlSanitizer;

        public HtmlSanitizerHelper(
            IHtmlSanitizer htmlSanitizer
            )
        {
            _htmlSanitizer = htmlSanitizer;
        }

        /// <summary>
        /// Takes raw HTML input and cleans against an allowlist to prevent
        /// XSS attacks
        /// </summary>
        /// <param name="source">Html source</param>
        public IHtmlContent Sanitize(string source)
        {
            return new HtmlString(_htmlSanitizer.Sanitize(source));
        }

        /// <summary>
        /// Takes raw HTML input and cleans against a allowlist to prevent
        /// XSS attacks
        /// </summary>
        /// <param name="source">Html source</param>
        public IHtmlContent Sanitize(IHtmlContent source)
        {
            return new HtmlString(_htmlSanitizer.Sanitize(source));
        }

        /// <summary>
        /// Takes a string and removes all HTML tags
        /// </summary>
        /// <param name="source">String content to sanitize</param>
        public string StripHtml(string source)
        {
            return _htmlSanitizer.StripHtml(source);
        }

        /// <summary>
        /// Takes a raw source and removes all HTML tags
        /// </summary>
        /// <param name="source">HtmlString content to sanitize</param>
        public string StripHtml(HtmlString source)
        {
            return _htmlSanitizer.StripHtml(source);
        }
    }
}
