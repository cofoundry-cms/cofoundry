namespace Cofoundry.Domain;

/// <summary>
/// Contains a small amount of custom entity identity and
/// page routing information. These route objects are cached 
/// in order to make routing lookups speedy.
/// </summary>
/// <inheritdoc/>
public class CustomEntityRoute : IPublishableEntity
{
    /// <summary>
    /// Unique 6 letter code representing the type of custom entity.
    /// </summary>
    public string CustomEntityDefinitionCode { get; set; }

    /// <summary>
    /// Database id of the custom entity.
    /// </summary>
    public int CustomEntityId { get; set; }

    /// <summary>
    /// Optional locale assigned to the custom entity
    /// if used in a localized site.
    /// </summary>
    public ActiveLocale Locale { get; set; }

    /// <summary>
    /// The string identifier slug which can
    /// be used as a lookup identifier or in the routing 
    /// of the custom entity page. Can be forced to be unique
    /// by a setting on the custom entity definition.
    /// </summary>
    public string UrlSlug { get; set; }

    public PublishStatus PublishStatus { get; set; }

    public DateTime? PublishDate { get; set; }

    /// <summary>
    /// The date and time that the entity was last published. This can be different to
    /// <see cref="PublishDate"/> which is generally the date the entity was originally
    /// published, with this property relecting any subsequent updates. The <see cref="PublishDate"/> 
    /// can be set manually to a future date when publishing, however the change is also 
    /// reflected in <see cref="LastPublishDate"/> if it is scheduled ahead of the existing 
    /// <see cref="LastPublishDate"/>.
    /// </summary>
    public DateTime? LastPublishDate { get; set; }

    public bool HasDraftVersion { get; set; }

    public bool HasPublishedVersion { get; set; }

    /// <summary>
    /// Optional ordering value applied to the custom entity 
    /// in relation to other custom entities with the same definition.
    /// </summary>
    public int? Ordering { get; set; }

    /// <summary>
    /// Routing information particular to specific versions.
    /// </summary>
    public ICollection<CustomEntityVersionRoute> Versions { get; set; }
}
