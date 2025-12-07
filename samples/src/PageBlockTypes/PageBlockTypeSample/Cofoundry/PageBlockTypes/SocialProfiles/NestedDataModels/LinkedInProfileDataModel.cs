using System.ComponentModel.DataAnnotations;

namespace PageBlockTypeSample;

[Display(Name = "LinkedIn")]
public class LinkedInProfileDataModel : INestedDataModel, ISocialProfileDataModel
{
    [Display(Name = "Profile Id")]
    [PreviewTitle]
    [Required]
    public string ProfileId { get; set; } = string.Empty;

    public string GetDescription()
    {
        return "LinkedIn";
    }

    public string? GetUrl()
    {
        if (string.IsNullOrWhiteSpace(ProfileId))
        {
            return null;
        }

        return "http://www.linkedin.com/in/" + ProfileId;
    }
}
