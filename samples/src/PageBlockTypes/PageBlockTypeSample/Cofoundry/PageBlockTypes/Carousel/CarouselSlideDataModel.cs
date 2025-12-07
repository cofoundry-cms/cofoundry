using System.ComponentModel.DataAnnotations;

namespace PageBlockTypeSample;

public class CarouselSlideDataModel : INestedDataModel
{
    [PreviewImage]
    [Display(Description = "Image to display as the background tot he slide.")]
    [Required]
    [Image]
    public int ImageId { get; set; }

    [PreviewTitle]
    [Required]
    [Display(Description = "Title to display in the slide.")]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Display(Description = "Formatted text to display in the slide.")]
    [Required]
    [Html(HtmlToolbarPreset.BasicFormatting)]
    public string Text { get; set; } = string.Empty;
}
