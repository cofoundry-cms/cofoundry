namespace Cofoundry.Web;

/// <summary>
/// Custom entity view model data used in a <see cref="ICustomEntityPageViewModel{TDisplayModel}"/>
/// implementation. This is similar to <see cref="CustomEntityRenderDetails"/> from the
/// domain, but adds a typed display model that is mapped from the raw custom entity data model.
/// </summary>
/// <typeparam name="TDisplayModel">
/// The type of view model used to represent the custom entity data model when formatted for display.
/// </typeparam>
public class CustomEntityRenderDetailsViewModel<TDisplayModel> : ICustomEntityRoutable
{
    /// <summary>
    /// The database id of the custom entity.
    /// </summary>
    public int CustomEntityId { get; set; }

    /// <summary>
    /// The database identifier for the version that this instance
    /// represents. The version may not always be the latest version and 
    /// is dependent on the query that was used to load this instance, typically 
    /// using a <see cref="PublishStatusQuery"/> value.
    /// </summary>
    public int CustomEntityVersionId { get; set; }

    /// <summary>
    /// Optional locale assigned to the custom entity
    /// if used in a localized site.
    /// </summary>
    public ActiveLocale? Locale { get; set; }

    /// <summary>
    /// The descriptive human-readable title of the custom entity.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The string identifier slug which can
    /// be used as a lookup identifier or in the routing 
    /// of the custom entity page. Can be forced to be unique
    /// by a setting on the custom entity definition.
    /// </summary>
    public string UrlSlug { get; set; } = string.Empty;

    /// <summary>
    /// The display model containing any custom data properties stored 
    /// against this entity. This is mapped from the <see cref="ICustomEntityDataModel"/>
    /// associated with the custom entity.
    /// </summary>
    public required TDisplayModel Model { get; set; }

    /// <summary>
    /// Page region and fully mapped block data for a specific page 
    /// template, which will have been specified in the query used to 
    /// load this instance.
    /// </summary>
    public IReadOnlyCollection<CustomEntityPageRegionRenderDetails> Regions { get; set; } = Array.Empty<CustomEntityPageRegionRenderDetails>();

    /// <summary>
    /// <see cref="WorkFlowStatus"/> of the version that this instance represents. The version
    /// may not always be the latest version and is dependent on the query that
    /// was used to load this instance, typically using a <see cref="PublishStatusQuery"/> value.
    /// </summary>
    public WorkFlowStatus WorkFlowStatus { get; set; }

    /// <summary>
    /// Indicates if the page is marked as published or not, which allows the page
    /// to be shown on the live site if the PublishDate has passed.
    /// </summary>
    public PublishStatus PublishStatus { get; set; }

    /// <summary>
    /// The date after which the page can be shown on the live site.
    /// </summary>
    public DateTime? PublishDate { get; set; }

    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public IReadOnlyCollection<string> PageUrls { get; set; } = Array.Empty<string>();
}
