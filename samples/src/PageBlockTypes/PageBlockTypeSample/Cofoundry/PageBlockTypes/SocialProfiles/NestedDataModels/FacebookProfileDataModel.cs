using System.ComponentModel.DataAnnotations;

namespace PageBlockTypeSample;

[Display(Name = "Facebook")]
public class FacebookProfileDataModel : INestedDataModel, ISocialProfileDataModel
{
    [Display(Name = "Facebook Id")]
    [PreviewTitle]
    [Required]
    public string FacebookId { get; set; } = string.Empty;

    public string GetDescription()
    {
        return "Facebook";
    }

    public string? GetUrl()
    {
        if (string.IsNullOrWhiteSpace(FacebookId))
        {
            return null;
        }

        return "https://www.facebook.com/" + FacebookId;
    }
}
