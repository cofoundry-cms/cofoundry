﻿namespace Cofoundry.Domain;

/// <summary>
/// Message published when the ordering of a custom entity has been changed. This
/// message is often published in a batch, so if you're handling this message it's
/// best to use <see cref="IBatchMessageHandler{CustomEntityOrderingUpdatedMessage}"/>.
/// </summary>
public class CustomEntityOrderingUpdatedMessage : ICustomEntityContentUpdatedMessage
{
    /// <summary>
    /// Ids of the custom entity that have had thier ordering changed
    /// </summary>
    public int CustomEntityId { get; set; }

    /// <summary>
    /// Definition code of the custom entity that the content change affects
    /// </summary>
    public string CustomEntityDefinitionCode { get; set; } = string.Empty;

    /// <summary>
    /// Marked as true because this is a batch operation and detecting which entities
    /// are published and which are not wasn't implemented. Can be fixed if this is becomes
    /// an issue.
    /// </summary>
    public bool HasPublishedVersionChanged
    {
        get { return true; }
    }
}
