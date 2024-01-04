namespace Cofoundry.Domain.Data;

/// <summary>
/// Lookup cache used for quickly finding the correct version for a
/// specific publish status query e.g. 'Latest', 'Published', 
/// 'PreferPublished'. These records are generated when custom entities
/// are published or unpublished.
/// </summary>
public class CustomEntityPublishStatusQuery
{
    /// <summary>
    /// Id of the custom entity this record represents. Forms a key
    /// with the PublishStatusQueryId.
    /// </summary>
    public int CustomEntityId { get; set; }

    /// <summary>
    /// Numeric representation of the domain PublishStatusQuery enum.
    /// </summary>
    public short PublishStatusQueryId { get; set; }

    /// <summary>
    /// The id of the version of the  custom entity that should be displayed
    /// for the corresponding PublishStatusQueryId.
    /// </summary>
    public int CustomEntityVersionId { get; set; }

    private CustomEntity? _customEntity;
    /// <summary>
    /// Custom entity that this record represents.
    /// </summary>
    public CustomEntity CustomEntity
    {
        get => _customEntity ?? throw NavigationPropertyNotInitializedException.Create<CustomEntity>(nameof(CustomEntity));
        set => _customEntity = value;
    }

    private CustomEntityVersion? _customEntityVersion;
    /// <summary>
    /// The version of the  custom entity that should be displayed
    /// for the corresponding PublishStatusQueryId.
    /// </summary>
    public CustomEntityVersion CustomEntityVersion
    {
        get => _customEntityVersion ?? throw NavigationPropertyNotInitializedException.Create<PagePublishStatusQuery>(nameof(CustomEntityVersion));
        set => _customEntityVersion = value;
    }

}
