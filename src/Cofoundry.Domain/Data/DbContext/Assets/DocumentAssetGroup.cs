namespace Cofoundry.Domain.Data;

[Obsolete("The document asset grouping system will be revised in an upcomming release.")]
public class DocumentAssetGroup : ICreateAuditable
{
    public int DocumentAssetGroupId { get; set; }

    public string GroupName { get; set; } = string.Empty;

    public int? ParentDocumentAssetGroupId { get; set; }

    public ICollection<DocumentAssetGroupItem> DocumentAssetGroupItems { get; set; } = new List<DocumentAssetGroupItem>();

    public ICollection<DocumentAssetGroup> ChildDocumentAssetGroups { get; set; } = new List<DocumentAssetGroup>();

    public DocumentAssetGroup? ParentDocumentAssetGroup { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<DocumentAssetGroup>(nameof(Creator));
        set;
    }
}
