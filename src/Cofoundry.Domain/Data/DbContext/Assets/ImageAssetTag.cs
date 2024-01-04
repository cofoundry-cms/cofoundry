namespace Cofoundry.Domain.Data;

public class ImageAssetTag : IEntityTag, ICreateAuditable
{
    public int ImageAssetId { get; set; }

    private ImageAsset? _imageAsset;
    public ImageAsset ImageAsset
    {
        get => _imageAsset ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetTag>(nameof(ImageAsset));
        set => _imageAsset = value;
    }

    public int TagId { get; set; }

    private Tag? _tag;
    public Tag Tag
    {
        get => _tag ?? throw NavigationPropertyNotInitializedException.Create<Tag>(nameof(Tag));
        set => _tag = value;
    }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetTag>(nameof(Creator));
        set => _creator = value;
    }
}
