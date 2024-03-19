﻿namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// A general-purpose projection of a custom entity with version 
/// specific data, including a deserialized data model.
/// </para>
/// <para>
/// This object is specific to a particular version which 
/// may not always be the latest (depending on the query)
/// </para>
/// </summary>
public class CustomEntityRenderSummary : ICustomEntityRoutable
{
    /// <summary>
    /// The database id of the custom entity.
    /// </summary>
    public int CustomEntityId { get; set; }

    /// <summary>
    /// The six character definition code that represents the type of custom
    /// entity e.g. Blog Post, Project, Product. The definition code is defined
    /// in a class that inherits from <see cref="ICustomEntityDefinition"/>.
    /// </summary>
    public string CustomEntityDefinitionCode { get; set; } = string.Empty;

    /// <summary>
    /// The database identifier for the version that this instance
    /// represents. The version may not always be the latest version and 
    /// is dependent on the query that was used to load this instance, typically 
    /// using a <see cref="PublishStatusQuery"/> value.
    /// </summary>
    public int CustomEntityVersionId { get; set; }

    /// <summary>
    /// A number representing any custom ordering applied to this custom 
    /// entity. Custom Entities do not have custom ordering by default and 
    /// the <see cref="IOrderableCustomEntityDefinition"/> interface should be used on your 
    /// definition to imply that custom ordering is available. Doing so will
    /// enable drag-drop ordering in the admin panel.
    /// </summary>
    public int? Ordering { get; set; }

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
    /// <see cref="WorkFlowStatus"/> of the version that this instance represents. The version
    /// may not always be the latest version and is dependent on the query that
    /// was used to load this instance, typically using a <see cref="PublishStatusQuery"/> value.
    /// </summary>
    public WorkFlowStatus WorkFlowStatus { get; set; }

    /// <summary>
    /// Indicates if the custom entity is marked as published or not, which allows the page
    /// to be shown on the live site if the PublishDate has passed.
    /// </summary>
    public PublishStatus PublishStatus { get; set; }

    /// <summary>
    /// The date after which the page can be shown on the live site.
    /// </summary>
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

    /// <summary>
    /// The deserialized data model object containing any custom
    /// data properties stored against this entity.
    /// </summary>
    public ICustomEntityDataModel Model { get; set; } = UninitializedCustomEntityDataModel.Instance;

    /// <summary>
    /// The date the custom entity was created.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// If this custom entity has page routes asspciated with it
    /// they will be included here. Typically you'd only expect a
    /// single page on a site to be associated with a custom entitiy 
    /// details, but it's technically possible to have many.
    /// </summary>
    public IReadOnlyCollection<string> PageUrls { get; set; } = Array.Empty<string>();
}
