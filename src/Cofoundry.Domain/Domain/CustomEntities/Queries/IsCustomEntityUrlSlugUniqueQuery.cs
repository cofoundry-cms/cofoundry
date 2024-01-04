namespace Cofoundry.Domain;

/// <summary>
/// Determines if the UrlSlug property for a custom entity is invalid because it
/// already exists. If the custom entity defition has ForceUrlSlugUniqueness 
/// set to true then duplicates are not permitted, otherwise true will always
/// be returned.
/// </summary>
public class IsCustomEntityUrlSlugUniqueQuery : IQuery<bool>
{
    /// <summary>
    /// Unique 6 character code representing the type of custom entity
    /// being checked for uniqueness.
    /// </summary>
    [Required]
    public string CustomEntityDefinitionCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional database id of a custom entity to exclude from the check. 
    /// Used when checking an existing custom entity for uniqueness.
    /// </summary>
    public int? CustomEntityId { get; set; }

    /// <summary>
    /// The string identifier slug to check for uniqueness. Null or empty 
    /// values are not valid, but will return <see langword="true"/> because 
    /// although uniqueness validation should not be triggered for these values
    /// it is technically the correct answer.
    /// </summary>
    public string? UrlSlug { get; set; }

    /// <summary>
    /// Optional id of the locale if used in a localized site.
    /// </summary>
    public int? LocaleId { get; set; }
}
