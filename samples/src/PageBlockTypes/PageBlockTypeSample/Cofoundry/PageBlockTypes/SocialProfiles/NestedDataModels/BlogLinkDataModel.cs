using System.ComponentModel.DataAnnotations;

namespace PageBlockTypeSample;

[Display(Name = "Blog")]
public class BlogLinkDataModel : INestedDataModel, ISocialProfileDataModel
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [PreviewTitle]
    [Url]
    [Required]
    public string Url { get; set; } = string.Empty;

    public string GetDescription()
    {
        return Description;
    }

    public string? GetUrl()
    {
        return Url;
    }
}
