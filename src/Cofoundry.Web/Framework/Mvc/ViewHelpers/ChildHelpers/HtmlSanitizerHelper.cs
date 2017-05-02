using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Web
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
        /// Takes raw HTML input and cleans against a whitelist to prevent
        /// XSS attacks
        /// </summary>
        /// <param name="source">Html source</param>
        public IHtmlContent Sanitize(string source)
        {
            return new HtmlString(_htmlSanitizer.Sanitize(source));
        }

        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist to prevent
        /// XSS attacks
        /// </summary>
        /// <param name="source">Html source</param>
        public IHtmlContent Sanitize(IHtmlContent source)
        {
            return new HtmlString(_htmlSanitizer.Sanitize(source));
        }
    }
}
