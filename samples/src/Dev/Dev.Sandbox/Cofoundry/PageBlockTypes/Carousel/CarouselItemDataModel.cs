using System.ComponentModel.DataAnnotations;

namespace Dev.Sandbox;

public class CarouselItemDataModel : INestedDataModel
{
    [PreviewTitle]
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [PreviewDescription]
    [Required]
    [MultiLineText]
    [MaxLength(200)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Url { get; set; } = string.Empty;

    [PreviewImage]
    [Image]
    public int ImageId { get; set; }
}
