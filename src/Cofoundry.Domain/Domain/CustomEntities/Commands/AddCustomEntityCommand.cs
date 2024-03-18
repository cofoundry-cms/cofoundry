﻿namespace Cofoundry.Domain;

/// <summary>
/// Adds a new custom entity with a draft version and optionally publishes it.
/// </summary>
public class AddCustomEntityCommand : ICommand, ILoggableCommand, ICustomEntityDataModelCommand, IValidatableObject
{
    /// <summary>
    /// Unique 6 character code representing the type of custom entity.
    /// </summary>
    [Required]
    [MaxLength(6)]
    public string CustomEntityDefinitionCode { get; set; } = string.Empty;

    /// <summary>
    /// The custom entity data model data to be validated and serialized 
    /// into the database.
    /// </summary>
    [Required]
    [ValidateObject]
    public ICustomEntityDataModel Model { get; set; } = null!;

    /// <summary>
    /// Optional locale id assigned to the custom entity
    /// if used in a localized site.
    /// </summary>
    [PositiveInteger]
    public int? LocaleId { get; set; }

    /// <summary>
    /// A url slug is usually required, except if the custom entity defintion
    /// has AutoUrlSlug enabled, in which case it is auto-generated.
    /// </summary>
    [MaxLength(200)]
    [Slug]
    public string? UrlSlug { get; set; }

    /// <summary>
    /// The descriptive human-readable title of the custom entity. If the 
    /// custom entity defintion has AutoUrlSlug enabled then this is used 
    /// to generate the UrlSlug.
    /// </summary>
    [MaxLength(200)]
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether to publish the custom entity immediately
    /// after adding it.
    /// </summary>
    public bool Publish { get; set; }

    /// <summary>
    /// If the Publish property is set to true then this optional field
    /// can be used to set a time that the custom entity should be published and 
    /// made public. If this is left null then the publish date is set to 
    /// the current date and the custom entity is made immediately available.
    /// </summary>
    public DateTime? PublishDate { get; set; }

    /// <summary>
    /// The database id of the newly created custom entity. This is set after the 
    /// command has been run.
    /// </summary>
    [OutputValue]
    public int OutputCustomEntityId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Publish && PublishDate.HasValue)
        {
            yield return new ValidationResult("A publish date can only be set when publishing.", new[] { nameof(PublishDate) });
        }
    }
}
