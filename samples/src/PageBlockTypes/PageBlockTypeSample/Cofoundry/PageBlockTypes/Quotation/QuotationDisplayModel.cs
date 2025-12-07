using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class QuotationDisplayModel : IPageBlockTypeDisplayModel
{
    public required string? Title { get; set; }
    public required IHtmlContent Quotation { get; set; }
    public required string? CitationText { get; set; }
    public required string? CitationUrl { get; set; }
}
