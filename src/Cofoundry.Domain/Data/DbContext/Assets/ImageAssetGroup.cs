namespace Cofoundry.Domain.Data;

[Obsolete("The image asset grouping system will be revised in an upcomming release.")]
public class ImageAssetGroup : ICreateAuditable
{
    public int ImageAssetGroupId { get; set; }

    public string GroupName { get; set; } = string.Empty;

    public int? ParentImageAssetGroupId { get; set; }

    public ImageAssetGroup? ParentImageAssetGroup { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetGroup>(nameof(Creator));
        set;
    }

    public ICollection<ImageAssetGroupItem> ImageAssetGroupItems { get; set; } = new List<ImageAssetGroupItem>();

    public ICollection<ImageAssetGroup> ChildImageAssetGroups { get; set; } = new List<ImageAssetGroup>();
}
