namespace Cofoundry.Domain.Data;

[Obsolete("The image asset grouping system will be revised in an upcomming release.")]
public class ImageAssetGroupItem : ICreateAuditable
{
    public int ImageAssetId { get; set; }

    public ImageAsset ImageAsset
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetGroupItem>(nameof(ImageAsset));
        set;
    }

    public int ImageAssetGroupId { get; set; }

    public ImageAssetGroup ImageAssetGroup
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetGroupItem>(nameof(ImageAssetGroup));
        set;
    }

    public int Ordering { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetGroupItem>(nameof(Creator));
        set;
    }
}
