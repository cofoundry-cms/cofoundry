using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web;

/// <summary>
/// Data model representing a quotation with optional citation and title
/// </summary>
public class QuotationDataModel : IPageBlockTypeDataModel
{
    [Display(Name = "Title (optional)")]
    [StringLength(128)]
    public string? Title { get; set; }

    [Required]
    [Display(Name = "Quotation text")]
    public string Quotation { get; set; } = string.Empty;

    [Display(Name = "Citation text (optional)")]
    [StringLength(128)]
    public string? CitationText { get; set; }

    [Display(Name = "Citation url")]
    [StringLength(256)]
    public string? CitationUrl { get; set; }
}
