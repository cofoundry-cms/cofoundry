namespace Cofoundry.Domain.Data;

[Obsolete("The image asset grouping system will be revised in an upcomming release.")]
public class ImageAssetGroupItem : ICreateAuditable
{
    public int ImageAssetId { get; set; }

    private ImageAsset? _imageAsset;
    public ImageAsset ImageAsset
    {
        get => _imageAsset ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetGroupItem>(nameof(ImageAsset));
        set => _imageAsset = value;
    }

    public int ImageAssetGroupId { get; set; }

    private ImageAssetGroup? _imageAssetGroup;
    public ImageAssetGroup ImageAssetGroup
    {
        get => _imageAssetGroup ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetGroupItem>(nameof(ImageAssetGroup));
        set => _imageAssetGroup = value;
    }

    public int Ordering { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetGroupItem>(nameof(Creator));
        set => _creator = value;
    }
}
