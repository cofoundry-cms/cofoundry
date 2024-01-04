namespace Cofoundry.Domain.Data;

public class DocumentAssetTag : ICreateAuditable, IEntityTag
{
    public int DocumentAssetId { get; set; }

    private DocumentAsset? _documentAsset;
    public DocumentAsset DocumentAsset
    {
        get => _documentAsset ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetTag>(nameof(DocumentAsset));
        set => _documentAsset = value;
    }

    public int TagId { get; set; }

    private Tag? _tag;
    public Tag Tag
    {
        get => _tag ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetTag>(nameof(Tag));
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
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetTag>(nameof(Creator));
        set => _creator = value;
    }
}
