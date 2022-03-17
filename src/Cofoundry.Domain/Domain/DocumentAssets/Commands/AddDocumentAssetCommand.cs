using System.Runtime.Serialization;

namespace Cofoundry.Domain;

/// <summary>
/// Adds a new document asset.
/// </summary>
public class AddDocumentAssetCommand : ICommand, ILoggableCommand, IValidatableObject
{
    /// <summary>
    /// The file source to retreive the document data from. The
    /// <see cref="IFileSource"/> abstraction is used here to support multiple
    /// types of file source e.g. FormFileSource, <see cref="EmbeddedResourceFileSource"/>.
    /// or <see cref="StreamFileSource"/>.
    /// </summary>
    [Required]
    [IgnoreDataMember]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [ValidateObject]
    public IFileSource File { get; set; }

    /// <summary>
    /// A short descriptive title of the document (130 characters).
    /// </summary>
    [StringLength(130)]
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// A longer description of the document in plain text.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Tags can be used to categorize an entity.
    /// </summary>
    public ICollection<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// The database id of the newly created document asset. This is set 
    /// after the command has been run.
    /// </summary>
    [OutputValue]
    public int OutputDocumentAssetId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return DocumentAssetCommandHelper.Validate(File);
    }
}
