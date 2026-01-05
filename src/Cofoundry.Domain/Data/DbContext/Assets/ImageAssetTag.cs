namespace Cofoundry.Domain.Data;

public class ImageAssetTag : IEntityTag, ICreateAuditable
{
    public int ImageAssetId { get; set; }

    public ImageAsset ImageAsset
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetTag>(nameof(ImageAsset));
        set;
    }

    public int TagId { get; set; }

    public Tag Tag
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<Tag>(nameof(Tag));
        set;
    }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<ImageAssetTag>(nameof(Creator));
        set;
    }
}
