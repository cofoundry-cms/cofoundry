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

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetGroup>(nameof(Creator));
        set => _creator = value;
    }

    public ICollection<ImageAssetGroupItem> ImageAssetGroupItems { get; set; } = new List<ImageAssetGroupItem>();

    public ICollection<ImageAssetGroup> ChildImageAssetGroups { get; set; } = new List<ImageAssetGroup>();
}
