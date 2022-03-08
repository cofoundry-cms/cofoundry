using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class HtmlSanitizerHelper : IHtmlSanitizerHelper
    {
        private readonly IHtmlSanitizer _htmlSanitizer;

        public HtmlSanitizerHelper(
            IHtmlSanitizer htmlSanitizer
            )
        {
            _htmlSanitizer = htmlSanitizer;
        }

        public IHtmlContent Sanitize(string source)
        {
            return new HtmlString(_htmlSanitizer.Sanitize(source));
        }

        public IHtmlContent Sanitize(IHtmlContent source)
        {
            return new HtmlString(_htmlSanitizer.Sanitize(source));
        }

        public string StripHtml(string source)
        {
            return _htmlSanitizer.StripHtml(source);
        }

        public string StripHtml(HtmlString source)
        {
            return _htmlSanitizer.StripHtml(source);
        }
    }
}