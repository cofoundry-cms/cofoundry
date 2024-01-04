namespace Cofoundry.Domain.Data;

[Obsolete("The document asset grouping system will be revised in an upcomming release.")]
public class DocumentAssetGroupItem : ICreateAuditable
{
    public int DocumentAssetId { get; set; }

    private DocumentAsset? _documentAsset;
    public DocumentAsset DocumentAsset
    {
        get => _documentAsset ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetGroupItem>(nameof(DocumentAsset));
        set => _documentAsset = value;
    }

    public int DocumentAssetGroupId { get; set; }

    private DocumentAssetGroup? _documentAssetGroup;
    public DocumentAssetGroup DocumentAssetGroup
    {
        get => _documentAssetGroup ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetGroupItem>(nameof(DocumentAssetGroup));
        set => _documentAssetGroup = value;
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
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetGroupItem>(nameof(Creator));
        set => _creator = value;
    }
}
