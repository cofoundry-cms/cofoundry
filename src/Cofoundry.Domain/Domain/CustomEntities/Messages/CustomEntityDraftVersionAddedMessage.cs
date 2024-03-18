﻿namespace Cofoundry.Domain;

/// <summary>
/// Message published when a new draft version has been added to a custom entity.
/// </summary>
public class CustomEntityDraftVersionAddedMessage
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
    /// Id of the newly creafted draft version
    /// </summary>
    public int CustomEntityVersionId { get; set; }
}
