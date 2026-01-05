namespace Cofoundry.Domain.Data;

public class DocumentAssetTag : ICreateAuditable, IEntityTag
{
    public int DocumentAssetId { get; set; }

    public DocumentAsset DocumentAsset
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetTag>(nameof(DocumentAsset));
        set;
    }

    public int TagId { get; set; }

    public Tag Tag
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetTag>(nameof(Tag));
        set;
    }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetTag>(nameof(Creator));
        set;
    }
}
