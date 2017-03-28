using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain
{
    public class PageSearchResult
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public HtmlString FoundText { get; set; }
    }
}
