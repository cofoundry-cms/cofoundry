namespace Cofoundry.Domain;

/// <summary>
/// An entity that used the standard publishing mechanism e.g.
/// a page or custom entity.
/// </summary>
public interface IPublishableEntity
{
    /// <summary>
    /// Indicates if the entity is marked as published or not, which allows the entity
    /// to be shown on the live site if the <see cref="PublishDate"/> has passed.
    /// </summary>
    PublishStatus PublishStatus { get; set; }

    /// <summary>
    /// The date that the entity was first published. This date can be set to a future date 
    /// to indicate that entity should not appear on the live site until this date has passed.
    /// </summary>
    DateTime? PublishDate { get; set; }

    /// <summary>
    /// Indicates whether there is a draft version of this entity available.
    /// </summary>
    bool HasDraftVersion { get; set; }

    /// <summary>
    /// Indicates whether there is a published version of this entity available.
    /// </summary>
    bool HasPublishedVersion { get; set; }
}
