using System.ComponentModel.DataAnnotations;

namespace PageBlockTypeSample;

public class SocialProfilesDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [Required]
    [NestedDataModelMultiTypeCollection(
        [
            typeof(FacebookProfileDataModel),
            typeof(TwitterProfileDataModel),
            typeof(LinkedInProfileDataModel),
            typeof(BlogLinkDataModel)
        ],
        IsOrderable = true,
        MinItems = 1,
        MaxItems = 10,
        TitleColumnHeader = "Profile"
        )]
    public IReadOnlyCollection<NestedDataModelMultiTypeItem> SocialProfiles { get; set; } = Array.Empty<NestedDataModelMultiTypeItem>();
}
