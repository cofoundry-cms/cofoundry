using System.Runtime.Serialization;

namespace Cofoundry.Domain;

/// <summary>
/// Adds a new image asset.
/// </summary>
public class AddImageAssetCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// The file source to retreive the image data from. The
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
    /// The title or alt text for an image. Recommended to be up 
    /// 125 characters to accomodate screen readers.
    /// </summary>
    [StringLength(130)]
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// The focal point to use when using dynamic cropping
    /// by default. 
    /// </summary>
    public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

    /// <summary>
    /// Tags can be used to categorize an entity.
    /// </summary>
    public ICollection<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// The database id of the newly created image asset. This is set 
    /// after the command has been run.
    /// </summary>
    [OutputValue]
    public int OutputImageAssetId { get; set; }
}
