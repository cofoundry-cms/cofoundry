﻿namespace Cofoundry.Domain;

/// <summary>
/// Message published when a new custom entity has been added.
/// </summary>
public class CustomEntityAddedMessage : ICustomEntityContentUpdatedMessage
{
    /// <summary>
    /// Id of the custom entity that the content change affects
    /// </summary>
    public int CustomEntityId { get; set; }

    /// <summary>
    /// Definition code of the custom entity that the content change affects
    /// </summary>
    public string CustomEntityDefinitionCode { get; set; } = string.Empty;

    /// <summary>
    /// True if the new custom entity was published
    /// </summary>
    public bool HasPublishedVersionChanged { get; set; }
}
