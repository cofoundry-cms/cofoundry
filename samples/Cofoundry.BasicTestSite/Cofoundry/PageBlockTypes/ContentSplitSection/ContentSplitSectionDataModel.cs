﻿using System.ComponentModel.DataAnnotations;

namespace Cofoundry.BasicTestSite;

public class ContentSplitSectionDataModel : IPageBlockTypeDataModel
{
    [Display(Description = "Optional title to display at the top of the section")]
    public string? Title { get; set; }

    [Required]
    [Display(Name = "Text", Description = "Rich text displayed alongside the image")]
    [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings)]
    public string HtmlText { get; set; } = string.Empty;

    [Display(Description = "Image to display alongside the text")]
    [Image(MinWidth = 1000, MinHeight = 1000)]
    public int ImageAssetId { get; set; }
}
