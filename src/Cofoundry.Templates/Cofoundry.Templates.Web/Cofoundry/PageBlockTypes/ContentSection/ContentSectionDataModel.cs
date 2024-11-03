using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Templates.Web;

/// <summary>
/// An example page block type. 
/// See https://www.cofoundry.org/docs/content-management/page-block-types
/// for more information
/// </summary>
public class ContentSectionDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [Display(Description = "Optional title to display at the top of the section")]
    public string? Title { get; set; }

    [Required]
    [Display(Name = "Text", Description = "Rich text displayed at full width")]
    [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings, HtmlToolbarPreset.Media)]
    public string HtmlText { get; set; } = string.Empty;
}
