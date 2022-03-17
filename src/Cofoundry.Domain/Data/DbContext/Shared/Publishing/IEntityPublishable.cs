namespace Cofoundry.Domain.Data;

/// <summary>
/// Represents an entity that can be published using standard publishing
/// features.
/// </summary>
public interface IEntityPublishable
{
    /// <summary>
    /// U = unpublished, P = Published. This is mapped to the
    /// <see cref="Cofoundry.Domain.PublishStatus"/> enum using
    /// <see cref="Cofoundry.Domain.Internal.PublishStatusMapper"/>.
    /// </summary>
    public string PublishStatusCode { get; set; }

    /// <summary>
    /// The date and time that the entity is or should be published.
    /// The publish date should always be set if the <see cref="PublishStatusCode"/> 
    /// is set to "P" (Published). Generally this tracks the first or original publish 
    /// date, with subsequent publishes only updating the <see cref="LastPublishDate"/>,
    /// however the <see cref="PublishDate"/> can be set to a specific date to allow for
    /// scheduled publishing.
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
}
