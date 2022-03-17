namespace Cofoundry.Domain;

/// <summary>
/// Message published when a custom entity has been deleted.
/// </summary>
public class CustomEntityDeletedMessage
{
    /// <summary>
    /// Id of the custom entity that the content change affects
    /// </summary>
    public int CustomEntityId { get; set; }

    /// <summary>
    /// Definition code of the custom entity that the content change affects
    /// </summary>
    public string CustomEntityDefinitionCode { get; set; }
}
