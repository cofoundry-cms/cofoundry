namespace Cofoundry.Domain.Data;

[Obsolete("The document asset grouping system will be revised in an upcomming release.")]
public class DocumentAssetGroupItem : ICreateAuditable
{
    public int DocumentAssetId { get; set; }

    public DocumentAsset DocumentAsset
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetGroupItem>(nameof(DocumentAsset));
        set;
    }

    public int DocumentAssetGroupId { get; set; }

    public DocumentAssetGroup DocumentAssetGroup
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetGroupItem>(nameof(DocumentAssetGroup));
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
        get => field ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetGroupItem>(nameof(Creator));
        set;
    }
}
