using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// An enhanced sanitization ruleset specific to the Ganss.XSS.HtmlSanitizer
    /// library. This allows for some additional configuration e.g. subscribing to 
    /// sanitization events.
    /// </summary>
    public interface IGanssHtmlSanitizationRuleSet : IHtmlSanitizationRuleSet
    {
        /// <summary>
        /// Performs additional initialization after the sanitizer has
        /// been created.
        /// </summary>
        /// <param name="htmlSanitizer">Sanitizer to initialize.</param>
        void Initialize(Ganss.XSS.HtmlSanitizer htmlSanitizer);

        /// <summary>
        /// The base URL relative URLs are resolved against. No resolution if empty.
        /// </summary>
        string BaseUrl { get; }
    }
}
