using Newtonsoft.Json;

namespace Cofoundry.Domain;

/// <summary>
/// Removes an image asset from the system and
/// queues any related files or caches to be removed
/// as a separate process.
/// </summary>
public class DeleteImageAssetCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// Database id of the image asset to remove.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int ImageAssetId { get; set; }
}
